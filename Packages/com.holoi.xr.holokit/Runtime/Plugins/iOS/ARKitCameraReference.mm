// SPDX-FileCopyrightText: Copyright 2026 Reality Design Lab <dev@reality.design>
// SPDX-License-Identifier: MIT
//
// From the HoloKit Unity SDK (Runtime/iOS/NativeCode/ARKitCameraReference.mm).
#import <ARKit/ARKit.h>

// XRSessionExtensions.GetNativePtr (ARKit XR Plugin): the ARSession behind the XR session subsystem.
typedef struct HoloKitCameraReferenceNativeSession
{
    int version;
    void* sessionPtr;
} HoloKitCameraReferenceNativeSession;

// UIKit's interface orientation in Unity's numbering (ScreenOrientation: Portrait 1, PortraitUpsideDown 2,
// LandscapeLeft 3, LandscapeRight 4), the one ARKit XR 6.7 renders with; 0 without a window scene.
extern "C" int HoloKit_GetInterfaceOrientation() {
    for (UIScene* scene in UIApplication.sharedApplication.connectedScenes) {
        if (![scene isKindOfClass:UIWindowScene.class]) continue;
        switch (((UIWindowScene*)scene).effectiveGeometry.interfaceOrientation) {
            case UIInterfaceOrientationPortrait: return 1;
            case UIInterfaceOrientationPortraitUpsideDown: return 2;
            case UIInterfaceOrientationLandscapeRight: return 3;
            case UIInterfaceOrientationLandscapeLeft: return 4;
            default: return 0;
        }
    }
    return 0;
}

// ARKit's camera pose, projection and background transform for one interface orientation (ARKitCameraReference.cs).
extern "C" int HoloKit_GetARCameraReference(void* nativeSession, int orientation,
    int width, int height, float* view, float* projection, float* display) {
    if (!nativeSession || orientation < 1 || orientation > 4) return 0;
    ARSession* session = (__bridge ARSession*)((HoloKitCameraReferenceNativeSession*)nativeSession)->sessionPtr;
    ARFrame* frame = session.currentFrame;
    if (!frame) return 0;
    // Unity's landscape names are the reverse of UIKit's (OrientationSupport.mm).
    UIInterfaceOrientation pose = orientation == 3 ? UIInterfaceOrientationLandscapeRight
        : orientation == 4 ? UIInterfaceOrientationLandscapeLeft
        : (UIInterfaceOrientation)orientation;
    simd_float4x4 v = [frame.camera viewMatrixForOrientation:pose];
    simd_float4x4 p = [frame.camera projectionMatrixForOrientation:pose
        viewportSize:CGSizeMake(width, height) zNear:.1 zFar:20];
    memcpy(view, &v, sizeof(v));
    memcpy(projection, &p, sizeof(p));
    CGAffineTransform d = [frame displayTransformForOrientation:pose viewportSize:CGSizeMake(width,height)];
    display[0]=d.a; display[1]=d.b; display[2]=d.c; display[3]=d.d; display[4]=d.tx; display[5]=d.ty;
    display[6]=CVPixelBufferGetWidth(frame.capturedImage);
    display[7]=CVPixelBufferGetHeight(frame.capturedImage);
    return 1;
}
