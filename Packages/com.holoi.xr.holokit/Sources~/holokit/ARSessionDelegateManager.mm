// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <ARKit/ARKit.h>
#import "UnityPluginApi/XR/UnityXRNativePtrs.h"
#import "HandTracker.h"
#import "Utils.h"

void (*OnThermalStateChanged)(int) = NULL;
void (*OnCameraChangedTrackingState)(int) = NULL;
void (*OnARWorldMapStatusChanged)(int) = NULL;
void (*OnGotCurrentARWorldMap)(void) = NULL;
void (*OnCurrentARWorldMapSaved)(const char *, int) = NULL;
void (*OnGotARWorldMapFromDisk)(bool, const char *, unsigned char *, int) = NULL;
void (*OnARWorldMapLoaded)(void) = NULL;
void (*OnRelocalizationSucceeded)(void) = NULL;
void (*OnARSessionUpdatedFrame)(double, const float *);
void (*OnARSessionUpdatedImageAnchor)(double, const float *);

typedef enum {
    VideoEnhancementModeNone = 0,
    VideoEnhancementModeHighRes = 1,
    VideoEnhancementModeHighResWithHDR = 2
} VideoEnhancementMode;

@interface ARSessionDelegateManager : NSObject

@end

@interface ARSessionDelegateManager() <ARSessionDelegate>

@property (nonatomic, weak, nullable) id <ARSessionDelegate> unityARSessionDelegate;
@property (nonatomic, strong, nullable) ARSession *arSession;
@property (nonatomic, strong, nullable) ARWorldMap *worldMap;
@property (assign) bool scaningEnvironment;
@property (assign) ARWorldMappingStatus currentARWorldMappingStatus;
@property (assign) bool sessionShouldAttemptRelocalization;
@property (assign) bool isRelocalizing;
@property (assign) VideoEnhancementMode videoEnhancementMode;
@property (assign) bool isNotFirstARSessionFrame;

@end

@implementation ARSessionDelegateManager

#pragma mark - init
- (instancetype)init {
    if (self = [super init]) {
        self.scaningEnvironment = NO;
        self.currentARWorldMappingStatus = ARWorldMappingStatusNotAvailable;
        self.sessionShouldAttemptRelocalization = NO;
        self.isRelocalizing = NO;
        self.videoEnhancementMode = VideoEnhancementModeNone;
        self.isNotFirstARSessionFrame = NO;
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(OnThermalStateChanged) name:NSProcessInfoThermalStateDidChangeNotification object:nil];
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

- (void)OnThermalStateChanged {
    if (OnThermalStateChanged != NULL) {
        NSProcessInfoThermalState thermalState = [[NSProcessInfo processInfo] thermalState];
        dispatch_async(dispatch_get_main_queue(), ^{
            OnThermalStateChanged((int)thermalState);
        });
    }
}

- (void)pauseCurrentARSession {
    if (self.arSession != nil) {
        [self.arSession pause];
    }
}

- (void)resumeCurrentARSession {
    if (self.arSession != nil) {
        [self.arSession runWithConfiguration:self.arSession.configuration];
    }
}

- (void)getCurentARWorldMap {
    if (self.currentARWorldMappingStatus != ARWorldMappingStatusMapped) {
        NSLog(@"[ARWorldMap] Current ARWorldMap is not available");
        return;
    }
    [self.arSession getCurrentWorldMapWithCompletionHandler:^(ARWorldMap * _Nullable worldMap, NSError * _Nullable error) {
        if (error != nil) {
            NSLog(@"[ARWorldMap] Failed to get current ARWorldMap");
            return;
        }
        self.worldMap = worldMap;
        if (OnGotCurrentARWorldMap != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnGotCurrentARWorldMap();
            });
        }
    }];
}

- (void)saveCurrentARWorldMapWithName:(NSString *)mapName {
    if (self.worldMap == nil) {
        NSLog(@"[ARSession] There is no current ARWorldMap");
        return;
    }
    
    NSData *mapData = [NSKeyedArchiver archivedDataWithRootObject:self.worldMap requiringSecureCoding:NO error:nil];
    // Create map folder if necessary
    NSString *directoryPath = [NSString stringWithFormat:@"%@%@", NSHomeDirectory(), @"/Documents/ARWorldMaps/"];
    if (![[NSFileManager defaultManager] fileExistsAtPath:directoryPath]) {
        [[NSFileManager defaultManager] createDirectoryAtPath:directoryPath withIntermediateDirectories:YES attributes:nil error:nil];
    }
    // Generate map file path
    NSString *filePath = [NSString stringWithFormat:@"%@%@%@", directoryPath, mapName, @".arexperience"];
    // Save map data to the path
    [mapData writeToFile:filePath atomically:YES];
    NSLog(@"[ARSessionController] Saved current ARWorldMapSaved with name %@ and size %lu bytes", mapName, mapData.length);
    if (OnCurrentARWorldMapSaved != NULL) {
        dispatch_async(dispatch_get_main_queue(), ^{
            OnCurrentARWorldMapSaved([mapName UTF8String], (int)mapData.length);
        });
    }
}

- (void)getARWorldMapFromDiskWithName:(NSString *)mapName mapSizeInBytes:(int)mapSizeInBytes {
    NSString *filePath = [NSString stringWithFormat:@"%@%@%@%@", NSHomeDirectory(), @"/Documents/ARWorldMaps/", mapName, @".arexperience"];
    if ([[NSFileManager defaultManager] fileExistsAtPath:filePath]) {
        NSData *mapData = [[NSData alloc] initWithContentsOfFile:filePath];
        if (mapData.length != mapSizeInBytes) {
            NSLog(@"[ARSessionController] Map on disk does not match the size");
            if (OnGotARWorldMapFromDisk != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnGotARWorldMapFromDisk(false, NULL, NULL, 0);
                });
            }
            return;
        }
        self.worldMap = nil;
        self.worldMap = [NSKeyedUnarchiver unarchivedObjectOfClass:[ARWorldMap class] fromData:mapData error:nil];
        if (self.worldMap == nil) {
            NSLog(@"[ARSessionController] Failed to get ARWorldMap from path: %@", filePath);
            if (OnGotARWorldMapFromDisk != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnGotARWorldMapFromDisk(false, NULL, NULL, 0);
                });
            }
            return;
        }
        NSLog(@"[ARSessionController] Got ARWorldMap %@ from disk with size %lu bytes", mapName, mapData.length);
        // Marshal map data back to C#
        unsigned char *data = (unsigned char *)[mapData bytes];
        if (OnGotARWorldMapFromDisk != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnGotARWorldMapFromDisk(true, [mapName UTF8String], data, (int)mapData.length);
            });
        }
    } else {
        NSLog(@"[ARSessionController] Failed to get ARWorldMap from path: %@", filePath);
        if (OnGotARWorldMapFromDisk != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnGotARWorldMapFromDisk(false, NULL, NULL, 0);
            });
        }
    }
}

- (void)nullifyCurrentARWorldMap {
    self.worldMap = nil;
}

- (void)loadARWorldMapWithData:(NSData *)mapData {
    self.worldMap = nil;
    self.worldMap = [NSKeyedUnarchiver unarchivedObjectOfClass:[ARWorldMap class] fromData:mapData error:nil];
    if (self.worldMap == nil) {
        NSLog(@"[ARSessionController] Load ARWorldMap with invalid map data");
        return;
    }
    if (OnARWorldMapLoaded != NULL) {
        dispatch_async(dispatch_get_main_queue(), ^{
            OnARWorldMapLoaded();
        });
    }
}

- (void)relocalizeToLoadedARWorldMap {
    if (self.worldMap == nil) {
        NSLog(@"[ARSessionController] There is no ARWorldMap to relocalize");
        return;
    }
    if (self.arSession == nil) {
        NSLog(@"[ARSessionController] There is no ARSession available");
        return;
    }
    ARWorldTrackingConfiguration *configuration = (ARWorldTrackingConfiguration *)self.arSession.configuration;
    configuration.initialWorldMap = self.worldMap;
    [self.arSession runWithConfiguration:configuration options:ARSessionRunOptionResetTracking|ARSessionRunOptionRemoveExistingAnchors];
    self.isRelocalizing = true;
}

- (void)updateVideoEnhancementMode:(VideoEnhancementMode)videoEnhancementMode {
    if (@available(iOS 16, *)) {
        ARWorldTrackingConfiguration *config = (ARWorldTrackingConfiguration *)self.arSession.configuration;
        if (ARWorldTrackingConfiguration.recommendedVideoFormatFor4KResolution == nil) {
            NSLog(@"[HoloKitSDK] Current device does not support 4K resolution video format");
            return;
        }
        
        if (videoEnhancementMode == VideoEnhancementModeNone) {
            //config.videoFormat = ARWorldTrackingConfiguration.supportedVideoFormats[0];
            NSLog(@"[HoloKitSDK] Default video format enabled");
            return;
        } else {
            config.videoFormat = ARWorldTrackingConfiguration.recommendedVideoFormatFor4KResolution;
            NSLog(@"[HoloKitSDK] 4K resolution video format enabled");
            if (videoEnhancementMode == VideoEnhancementModeHighResWithHDR) {
                if (!config.videoFormat.videoHDRSupported) {
                    NSLog(@"[HoloKitSDK] Current device does not support HDR video format");
                    return;
                }
                config.videoHDRAllowed = true;
                NSLog(@"[HoloKitSDK] HDR video format enabled");
            }
        }
        [self.arSession runWithConfiguration:config];
    } else {
        NSLog(@"[HoloKitSDK] Video enhancement is only supported on iOS 16");
    }
}

#pragma mark - ARSessionDelegate

- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame {
    if (self.unityARSessionDelegate != NULL) {
        [self.unityARSessionDelegate session:session didUpdateFrame:frame];
    }
    
    if (!self.isNotFirstARSessionFrame) {
        NSLog(@"[HoloKitSDK] This is the first frame of the current ARSession");
        self.isNotFirstARSessionFrame = YES;
        [self updateVideoEnhancementMode:self.videoEnhancementMode];
    }
    
    if (OnARSessionUpdatedFrame != NULL) {
        float *matrix = [Utils getUnityMatrix:frame.camera.transform];
        double timestamp = frame.timestamp;
        dispatch_async(dispatch_get_main_queue(), ^{
            OnARSessionUpdatedFrame(timestamp, matrix);
            delete[](matrix);
        });
    }
    
    // ARWorldMap status
//    if (self.scaningEnvironment) {
//        if (self.currentARWorldMappingStatus != frame.worldMappingStatus) {
//            switch (self.currentARWorldMappingStatus) {
//                case ARWorldMappingStatusNotAvailable:
//                    NSLog(@"[ARSessionController] Current ARWorldMapStatus changed to not available");
//                    break;
//                case ARWorldMappingStatusLimited:
//                    NSLog(@"[ARSessionController] Current ARWorldMapStatus changed to limited");
//                    break;
//                case ARWorldMappingStatusExtending:
//                    NSLog(@"[ARSessionController] Current ARWorldMapStatus changed to extending");
//                    break;
//                case ARWorldMappingStatusMapped:
//                    NSLog(@"[ARSessionController] Current ARWorldMapStatus changed to mapped");
//                    break;
//            }
//            self.currentARWorldMappingStatus = frame.worldMappingStatus;
//            if (OnARWorldMapStatusChanged != NULL) {
//                dispatch_async(dispatch_get_main_queue(), ^{
//                    OnARWorldMapStatusChanged((int)self.currentARWorldMappingStatus);
//                });
//            }
//        }
//    }
    
    // Hand tracking
    HandTracker *handTracker = [HandTracker sharedInstance];
    if ([handTracker active]) {
        [handTracker performHumanHandPoseRequest:frame];
    }
}

- (void)session:(ARSession *)session didAddAnchors:(NSArray<__kindof ARAnchor*>*)anchors {
    if (self.unityARSessionDelegate != NULL) {
        [self.unityARSessionDelegate session:session didAddAnchors:anchors];
    }
}

- (void)session:(ARSession *)session didUpdateAnchors:(NSArray<__kindof ARAnchor*>*)anchors {
    if (self.unityARSessionDelegate) {
        [self.unityARSessionDelegate session:session didUpdateAnchors:anchors];
    }
    
//    for (ARAnchor *anchor in anchors) {
//        if ([anchor isKindOfClass:[ARImageAnchor class]]) {
//            if (OnARSessionUpdatedImageAnchor != NULL) {
//                float *matrix = [Utils getUnityMatrix:anchor.transform];
//                double timestamp = session.currentFrame.timestamp;
//                dispatch_async(dispatch_get_main_queue(), ^{
//                    OnARSessionUpdatedImageAnchor(timestamp, matrix);
//                    delete[](matrix);
//                });
//            }
//            // There should only be one image anchor
//            return;
//        }
//    }
}
    
- (void)session:(ARSession *)session didRemoveAnchors:(NSArray<__kindof ARAnchor*>*)anchors {
    if (self.unityARSessionDelegate) {
        [self.unityARSessionDelegate session:session didRemoveAnchors:anchors];
    }
}

- (void)session:(ARSession *)session didOutputCollaborationData:(ARCollaborationData *)data {
    if (self.unityARSessionDelegate) {
        [self.unityARSessionDelegate session:session didOutputCollaborationData:data];
    }
}

#pragma mark - ARSessionObserver

- (void)session:(ARSession *)session cameraDidChangeTrackingState:(ARCamera *)camera {
    switch (camera.trackingState) {
        case ARTrackingStateNotAvailable:
            NSLog(@"[ARSessionObserver] Camera tracking state changed to not available");
            if (OnCameraChangedTrackingState != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnCameraChangedTrackingState(0);
                });
            }
            break;
        case ARTrackingStateLimited:
            switch(camera.trackingStateReason) {
                case ARTrackingStateReasonNone:
                    NSLog(@"[ARSessionObserver] Camera tracking state changed to limited, with reason: None");
                    if (OnCameraChangedTrackingState != NULL) {
                        dispatch_async(dispatch_get_main_queue(), ^{
                            OnCameraChangedTrackingState(1);
                        });
                    }
                    break;
                case ARTrackingStateReasonInitializing:
                    NSLog(@"[ARSessionObserver] Camera tracking state changed to limited, with reason: Initializing");
                    if (OnCameraChangedTrackingState != NULL) {
                        dispatch_async(dispatch_get_main_queue(), ^{
                            OnCameraChangedTrackingState(2);
                        });
                    }
                    break;
                case ARTrackingStateReasonExcessiveMotion:
                    NSLog(@"[ARSessionObserver] Camera tracking state changed to limited, with reason: Excessive motion");
                    if (OnCameraChangedTrackingState != NULL) {
                        dispatch_async(dispatch_get_main_queue(), ^{
                            OnCameraChangedTrackingState(3);
                        });
                    }
                    break;
                case ARTrackingStateReasonInsufficientFeatures:
                    NSLog(@"[ARSessionObserver] Camera tracking state changed to limited, with reason: Insufficient features");
                    if (OnCameraChangedTrackingState != NULL) {
                        dispatch_async(dispatch_get_main_queue(), ^{
                            OnCameraChangedTrackingState(4);
                        });
                    }
                    break;
                case ARTrackingStateReasonRelocalizing:
                    NSLog(@"[ARSessionObserver] Camera tracking state changed to limited, with reason: Relocalizing");
                    if (OnCameraChangedTrackingState != NULL) {
                        dispatch_async(dispatch_get_main_queue(), ^{
                            OnCameraChangedTrackingState(5);
                        });
                    }
                    break;
            }
            break;
        case ARTrackingStateNormal:
            NSLog(@"[ARSessionObserver] Camera tracking state changed to normal");
            if (OnCameraChangedTrackingState != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnCameraChangedTrackingState(6);
                });
            }
            if (self.isRelocalizing) {
                if (OnRelocalizationSucceeded != NULL) {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        OnRelocalizationSucceeded();
                    });
                }
                self.isRelocalizing = false;
            }
            break;
    }
}

- (void)sessionWasInterrupted:(ARSession *)session {
    NSLog(@"[ARSessionObserver] Session was interrupted");
}

- (void)sessionInterruptionEnded:(ARSession *)session {
    NSLog(@"[ARSessionObserver] Session interruption ended");
}

- (BOOL)sessionShouldAttemptRelocalization:(ARSession *)session {
    NSLog(@"[ARSessionObserver] SessionShouldAttemptRelocalization %d", self.sessionShouldAttemptRelocalization);
    return self.sessionShouldAttemptRelocalization;
}

@end

extern "C" {

void HoloKitSDK_InterceptUnityARSessionDelegate(UnityXRNativeSession* nativeARSessionPtr) {
    if (nativeARSessionPtr == NULL) {
        NSLog(@"[HoloKitSDK] Native ARSession is NULL");
        return;
    }
    ARSession* sessionPtr = (__bridge ARSession*)nativeARSessionPtr->sessionPtr;
    ARSessionDelegateManager* arSessionDelegateController = [ARSessionDelegateManager sharedInstance];
    arSessionDelegateController.unityARSessionDelegate = sessionPtr.delegate;
    [arSessionDelegateController setArSession:sessionPtr];
    [sessionPtr setDelegate:arSessionDelegateController];
}

int HoloKitSDK_GetThermalState(void) {
    NSProcessInfoThermalState thermalState = [[NSProcessInfo processInfo] thermalState];
    return (int)thermalState;
}

void HoloKitSDK_PauseCurrentARSession(void) {
    [[ARSessionDelegateManager sharedInstance] pauseCurrentARSession];
}

void HoloKitSDK_ResumeCurrentARSession(void) {
    [[ARSessionDelegateManager sharedInstance] resumeCurrentARSession];
}

void HoloKitSDK_SetSessionShouldAttemptRelocalization(bool value) {
    [[ARSessionDelegateManager sharedInstance] setSessionShouldAttemptRelocalization:value];
}

void HoloKitSDK_SetScaningEnvironment(bool value) {
    [[ARSessionDelegateManager sharedInstance] setScaningEnvironment:value];
}

void HoloKitSDK_GetCurrentARWorldMap(void) {
    [[ARSessionDelegateManager sharedInstance] getCurentARWorldMap];
}

void HoloKitSDK_SaveCurrentARWorldMapWithName(const char *mapName) {
    [[ARSessionDelegateManager sharedInstance] saveCurrentARWorldMapWithName:[NSString stringWithUTF8String:mapName]];
}

void HoloKitSDK_GetARWorldMapFromDiskWithName(const char *mapName, int mapSizeInBytes) {
    [[ARSessionDelegateManager sharedInstance] getARWorldMapFromDiskWithName:[NSString stringWithUTF8String:mapName] mapSizeInBytes:mapSizeInBytes];
}

void HoloKitSDK_NullifyCurrentARWorldMap(void) {
    [[ARSessionDelegateManager sharedInstance] nullifyCurrentARWorldMap];
}

void HoloKitSDK_LoadARWorldMapWithData(unsigned char *mapData, int dataSizeInBytes) {
    NSData *nsMapData = [NSData dataWithBytes:mapData length:dataSizeInBytes];
    [[ARSessionDelegateManager sharedInstance] loadARWorldMapWithData:nsMapData];
}

void HoloKitSDK_RelocalizeToLoadedARWorldMap(void) {
    [[ARSessionDelegateManager sharedInstance] relocalizeToLoadedARWorldMap];
}

void HoloKitSDK_SetVideoEnhancementMode(int mode) {
    [[ARSessionDelegateManager sharedInstance] setVideoEnhancementMode:(VideoEnhancementMode)mode];
}

void HoloKitSDK_RegisterARSessionControllerDelegates(void (*OnThermalStateChangedDelegate)(int),
                                                     void (*OnCameraChangedTrackingStateDelegate)(int),
                                                     void (*OnARWorldMapStatusChangedDelegate)(int),
                                                     void (*OnGotCurrentARWorldMapDelegate)(void),
                                                     void (*OnCurrentARWorldMapSavedDelegate)(const char *, int),
                                                     void (*OnGotARWorldMapFromDiskDelegate)(bool, const char *, unsigned char *, int),
                                                     void (*OnARWorldMapLoadedDelegate)(void),
                                                     void (*OnRelocalizationSucceededDelegate)(void),
                                                     void (*OnARSessionUpdatedFrameDelegate)(double, const float *),
                                                     void (*OnARSessionUpdatedImageAnchorDelegate)(double, const float *)) {
    OnThermalStateChanged = OnThermalStateChangedDelegate;
    OnCameraChangedTrackingState = OnCameraChangedTrackingStateDelegate;
    OnARWorldMapStatusChanged = OnARWorldMapStatusChangedDelegate;
    OnGotCurrentARWorldMap = OnGotCurrentARWorldMapDelegate;
    OnCurrentARWorldMapSaved = OnCurrentARWorldMapSavedDelegate;
    OnGotARWorldMapFromDisk = OnGotARWorldMapFromDiskDelegate;
    OnARWorldMapLoaded = OnARWorldMapLoadedDelegate;
    OnRelocalizationSucceeded = OnRelocalizationSucceededDelegate;
    OnARSessionUpdatedFrame = OnARSessionUpdatedFrameDelegate;
    OnARSessionUpdatedImageAnchor = OnARSessionUpdatedImageAnchorDelegate;
}

void HoloKitSDK_ResetOrigin(float position[3], float rotation[4]) {
    simd_float4x4 transform_matrix = [Utils getSimdFloat4x4WithPosition:position rotation:rotation];
    [[[ARSessionDelegateManager sharedInstance] arSession] setWorldOrigin:transform_matrix];
}

double HoloKitSDK_GetSystemUptime(void) {
    return [[NSProcessInfo processInfo] systemUptime];
}

void HoloKitSDK_ResetARSessionFirstFrame() {
    [[ARSessionDelegateManager sharedInstance] setIsNotFirstARSessionFrame:NO];
}

double HoloKitSDK_GetScreenBrightness() {
    return [[UIScreen mainScreen] brightness];
}

void HoloKitSDK_SetScreenBrightness(double value) {
    [[UIScreen mainScreen] setBrightness:value];
}
 
}
