// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import "HandTracker.h"

static const float kMaxLandmarkDepth = 0.6f;
static const float kMaxLandmarkStartInterval = 0.12f;
static const float kMaxLandmark1Interval = 0.05f;
static const float kMaxLandmark2Interval = 0.03f;
static const float kMaxLandmarkEndInterval = 0.024f;

void (*OnHandPoseUpdated)(float *) = NULL;

@interface HandTracker()

@property (nonatomic, strong) VNDetectHumanHandPoseRequest *handPoseRequest;

@end

@implementation HandTracker

- (instancetype)init {
    if (self = [super init]) {
        self.handPoseRequest = [[VNDetectHumanHandPoseRequest alloc] init];
        self.handPoseRequest.maximumHandCount = 1;
        self.active = false;
    }
    return self;
}

+ (id)sharedInstance {
    static dispatch_once_t onceToken = 0;
    static id _sharedObject = nil;
    dispatch_once(&onceToken, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (void)performHumanHandPoseRequest:(ARFrame *)frame {
    if (frame.sceneDepth == nil) {
        NSLog(@"[HandTracking] Failed to get scene depth");
        return;
    }
    
    VNImageRequestHandler *requestHandler = [[VNImageRequestHandler alloc]
                                             initWithCVPixelBuffer: frame.capturedImage
                                             orientation:kCGImagePropertyOrientationUp options:[NSMutableDictionary dictionary]];
    @try {
        NSArray<VNRequest *> * requests = [[NSArray alloc] initWithObjects:self.handPoseRequest, nil];
        [requestHandler performRequests:requests error:nil];
        if (self.handPoseRequest.results.count == 0) {
            return;
        }
        
        // Scene Depth
        CVPixelBufferLockBaseAddress(frame.sceneDepth.depthMap, 0);
        size_t depthBufferWidth = CVPixelBufferGetWidth(frame.sceneDepth.depthMap);
        size_t depthBufferHeight = CVPixelBufferGetHeight(frame.sceneDepth.depthMap);
        Float32 *depthBufferBaseAddress = (Float32 *)CVPixelBufferGetBaseAddress(frame.sceneDepth.depthMap);

        VNHumanHandPoseObservation *handPoseObservation = self.handPoseRequest.results[0];
        NSDictionary<VNRecognizedPointKey, VNRecognizedPoint*>* landmarks = [handPoseObservation recognizedPointsForGroupKey:VNRecognizedPointGroupKeyAll error:nil];
        float landmarkDepths[21];
        float *resultLandmarks = new float[63];
        for (int landmarkIndex = 0; landmarkIndex < 21; landmarkIndex++) {
            VNRecognizedPointKey key = [HandTracker landmarkIndexToHumanHandPoseKey:landmarkIndex];
            VNRecognizedPoint *landmark = [landmarks objectForKey:key];
            // Calculate the coordinate of this point in depth buffer space.
            int depthX = landmark.x * depthBufferWidth;
            int depthY = (1 - landmark.y) * depthBufferHeight;
            float landmarkDepth = (float)depthBufferBaseAddress[depthY * depthBufferWidth + depthX];
            // Depth validation to eliminate false positive results.
            if (landmarkIndex == 0 && landmarkDepth > kMaxLandmarkDepth) {
                // The depth of the wrist is not reasonable, which means that
                // this result is false positive, abandon it.
                return;
            }
            if (landmarkIndex != 0) {
                int landmarkParentIndex = [HandTracker getParentLandmarkIndex:landmarkIndex];
                if (landmarkDepth > kMaxLandmarkDepth) {
                    landmarkDepth = landmarkDepths[landmarkParentIndex];
                }
                if (landmarkIndex == 1 || landmarkIndex == 5 || landmarkIndex == 9 || landmarkIndex == 13 || landmarkIndex == 17) {
                    if (fabsf(landmarkDepth - landmarkDepths[landmarkParentIndex]) > kMaxLandmarkStartInterval) {
                        landmarkDepth = landmarkDepths[landmarkParentIndex];
                    }
                } else if (landmarkIndex == 2 || landmarkIndex == 6 || landmarkIndex == 10 || landmarkIndex == 14 || landmarkIndex == 18) {
                    if (fabsf(landmarkDepth - landmarkDepths[landmarkParentIndex]) > kMaxLandmark1Interval) {
                        landmarkDepth = landmarkDepths[landmarkParentIndex];
                    }
                } else if (landmarkIndex == 3 || landmarkIndex == 7 || landmarkIndex == 11 || landmarkIndex == 15 || landmarkIndex == 19) {
                    if (fabsf(landmarkDepth - landmarkDepths[landmarkParentIndex]) > kMaxLandmark2Interval) {
                        landmarkDepth = landmarkDepths[landmarkParentIndex];
                    }
                } else {
                    if (fabsf(landmarkDepth - landmarkDepths[landmarkParentIndex]) > kMaxLandmarkEndInterval) {
                        landmarkDepth = landmarkDepths[landmarkParentIndex];
                    }
                }
            }
            landmarkDepths[landmarkIndex] = landmarkDepth;
            // Landmark's x and y coordinate are originated from bottom-left corner and between 0 and 1.
            // Calculte the screen space coordinate of this point.
            CGFloat screenX = (CGFloat)landmark.x * frame.camera.imageResolution.width;
            CGFloat screenY = (CGFloat)(1 - landmark.y) * frame.camera.imageResolution.height;
            CGPoint screenPoint = CGPointMake(screenX, screenY);
            simd_float3 unprojectedPoint = [HandTracker unprojectScreenPoint:screenPoint depth:landmarkDepth frame:frame];
            resultLandmarks[landmarkIndex * 3 + 0] = unprojectedPoint.x;
            resultLandmarks[landmarkIndex * 3 + 1] = unprojectedPoint.y;
            resultLandmarks[landmarkIndex * 3 + 2] = -unprojectedPoint.z;
        }
        CVPixelBufferUnlockBaseAddress(frame.sceneDepth.depthMap, 0);
        if (OnHandPoseUpdated != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnHandPoseUpdated(resultLandmarks);
                delete[](resultLandmarks);
            });
        }
    } @catch(NSException * e) {
        NSLog(@"[HandTracking] Human hand pose request failed");
    }
}

+ (simd_float3)unprojectScreenPoint:(CGPoint)screenPoint depth:(float)z frame:(ARFrame *)frame {
    simd_float4x4 translation = matrix_identity_float4x4;
    translation.columns[3].z = -z;
    simd_float4x4 planeOrigin = simd_mul(frame.camera.transform, translation);
    simd_float3 xAxis = simd_make_float3(1, 0, 0);
    simd_float4x4 rotation = simd_matrix4x4(simd_quaternion(0.5 * M_PI, xAxis));
    simd_float4x4 plane = simd_mul(planeOrigin, rotation);
    simd_float3 unprojectedPoint = [frame.camera unprojectPoint:screenPoint ontoPlaneWithTransform:plane orientation:UIInterfaceOrientationLandscapeRight viewportSize:frame.camera.imageResolution];
    return unprojectedPoint;
}

+ (int)getParentLandmarkIndex:(int)landmarkIndex {
    int parentIndex;
    if (landmarkIndex == 0 || landmarkIndex == 5 || landmarkIndex == 9 || landmarkIndex == 13 || landmarkIndex == 17) {
        parentIndex = 0;
    } else{
        parentIndex = landmarkIndex - 1;
    }
    return parentIndex;
}

+ (VNRecognizedPointKey)landmarkIndexToHumanHandPoseKey:(int)landmarkIndex {
    switch(landmarkIndex) {
        case 0:
            return VNHumanHandPoseObservationJointNameWrist;
        case 1:
            return VNHumanHandPoseObservationJointNameThumbCMC;
        case 2:
            return VNHumanHandPoseObservationJointNameThumbMP;
        case 3:
            return VNHumanHandPoseObservationJointNameThumbIP;
        case 4:
            return VNHumanHandPoseObservationJointNameThumbTip;
        case 5:
            return VNHumanHandPoseObservationJointNameIndexMCP;
        case 6:
            return VNHumanHandPoseObservationJointNameIndexPIP;
        case 7:
            return VNHumanHandPoseObservationJointNameIndexDIP;
        case 8:
            return VNHumanHandPoseObservationJointNameIndexTip;
        case 9:
            return VNHumanHandPoseObservationJointNameMiddleMCP;
        case 10:
            return VNHumanHandPoseObservationJointNameMiddlePIP;
        case 11:
            return VNHumanHandPoseObservationJointNameMiddleDIP;
        case 12:
            return VNHumanHandPoseObservationJointNameMiddleTip;
        case 13:
            return VNHumanHandPoseObservationJointNameRingMCP;
        case 14:
            return VNHumanHandPoseObservationJointNameRingPIP;
        case 15:
            return VNHumanHandPoseObservationJointNameRingDIP;
        case 16:
            return VNHumanHandPoseObservationJointNameRingTip;
        case 17:
            return VNHumanHandPoseObservationJointNameLittleMCP;
        case 18:
            return VNHumanHandPoseObservationJointNameLittlePIP;
        case 19:
            return VNHumanHandPoseObservationJointNameLittleDIP;
        case 20:
            return VNHumanHandPoseObservationJointNameLittleTip;
        default:
            return VNHumanHandPoseObservationJointNameLittleTip;
    }
}

@end

extern "C" {

void HoloKitSDK_SetHandTrackingActive(bool active) {
    [[HandTracker sharedInstance] setActive:active];
}

void HoloKitSDK_RegisterHandTrackingControllerDelegates(void (*OnHandPoseUpdatedDelegate)(float *)) {
    OnHandPoseUpdated = OnHandPoseUpdatedDelegate;
}

}
