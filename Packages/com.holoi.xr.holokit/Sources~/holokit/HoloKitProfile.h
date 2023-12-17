// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <Foundation/Foundation.h>
#import <simd/simd.h>

typedef enum {
    HoloKitX = 0
} HoloKitType;

typedef enum {
    iPhoneXS = 0,
    iPhoneXSMax = 1,
    iPhone11Pro = 2,
    iPhone11ProMax = 3,
    iPhone12mini = 4,
    iPhone12 = 5,
    iPhone12Pro = 6,
    iPhone12ProMax = 7,
    iPhone13mini = 8,
    iPhone13 = 9,
    iPhone13Pro = 10,
    iPhone13ProMax = 11,
    iPhone14 = 12,
    iPhone14Plus = 13,
    iPhone14Pro = 14,
    iPhone14ProMax = 15,
    iPhone15 = 16,
    iPhone15Plus = 17,
    iPhone15Pro = 18,
    iPhone15ProMax = 19,
    iPadWithLidar = 101, // all iPads with Lidar
    Unknown = 999
} PhoneType;

typedef struct {
    // Distance beetween eyes
    float OpticalAxisDistance;
    
    // 3D offset from the center of bottomline of the holokit phone display to the center of two eyes.
    // Left-handedness
    simd_float3 MrOffset;
    
    // Eye view area width
    float ViewportInner;
    
    // Eye view area height
    float ViewportOuter;
    
    // Eye view area spillter width
    float ViewportTop;
    
    // Eye view area spillter width
    float ViewportBottom;
    
    // Fresnel lens focal length
    float FocalLength;
    
    // Screen To Fresnel distance
    float ScreenToLens;
    
    // Fresnel To eye distance
    float LensToEye;
    
    // Bottom of the holder to bottom of the view
    float AxisToBottom;
    
    // The distance between the center of the HME and the marker
    float HorizontalAlignmentMarkerOffset;
} HoloKitModel;

typedef struct {
    // The long screen edge of the phone
    float ScreenWidth;
    
    // The short screen edge of the phone
    float ScreenHeight;
    
    // The distance from the bottom of display area to the touching surface of the holokit phone holder
    float ScreenBottom;
    
    // The 3D offset vector from center of the camera to the center of the display area's bottomline
    // Left-handedness
    simd_float3 CameraOffset;
    
    float ScreenDpi;
} PhoneModel;

@interface HoloKitProfile : NSObject

+ (HoloKitModel)getHoloKitModel:(HoloKitType)holokitType;

+ (PhoneModel)getPhoneModel;

+ (BOOL)IsCurrentDeviceSupportedByHoloKit;

+ (BOOL)IsCurrentDeviceIpad;

+ (BOOL)IsCurrentDeviceEquippedWithLiDAR;

@end
