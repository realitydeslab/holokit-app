#pragma once

//https://github.com/chenjd/Unity-ARFoundation-HandDetection

// Various AR Subsystems have GetNativePtr methods on them, which return
// pointers to the following structs. The first field will always
// be a version number, so code which tries to interpret the native
// pointers can safely check the version prior to casting to the
// appropriate struct.

// XRSessionExtensions.GetNativePtr
typedef struct UnityXRNativeSession
{
    int version;
    void* sessionPtr;
} UnityXRNativeSession;

typedef struct UnityXRNativeFrame
{
    int version;
    void* framePtr;
} UnityXRNativeFrame;

// XRPlaneExtensions.GetNativePtr
typedef struct UnityXRNativePlane
{
    int version;
    void* planePtr;
} UnityXRNativePlane;

// XRReferencePointExtensions.GetNativePtr
typedef struct UnityXRNativeReferencePoint
{
    int version;
    void* referencePointPtr;
} UnityXRNativeReferencePoint;

typedef struct UnityXRNativePointCloud
{
    int version;
    void* pointCloud;
} UnityXRNativePointCloud;

typedef struct UnityXRNativeImage
{
    int version;
    void* imageTrackable;
} UnityXRNativeImage;

static const int kUnityXRNativeSessionVersion = 1;
static const int kUnityXRNativeFrameVersion = 1;
static const int kUnityXRNativePlaneVersion = 1;
static const int kUnityXRNativeReferencePointVersion = 1;
static const int kUnityXRNativePointCloudVersion = 1;
static const int kUnityXRNativeImageVersion = 1;

