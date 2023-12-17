// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import "HoloKitProfile.h"
#import <sys/utsname.h>

@interface HoloKitProfile()

@end

@implementation HoloKitProfile

+ (HoloKitModel)getHoloKitModel:(HoloKitType)holokitType {
    HoloKitModel holokitModel;
    switch (holokitType) {
        case HoloKitX:
            holokitModel.OpticalAxisDistance = 0.064;
            holokitModel.MrOffset = simd_make_float3(0, -0.02894, -0.07055);
            holokitModel.ViewportInner = 0.0292;
            holokitModel.ViewportOuter = 0.0292;
            holokitModel.ViewportTop = 0.02386;
            holokitModel.ViewportBottom = 0.02386;
            holokitModel.FocalLength = 0.065;
            holokitModel.ScreenToLens = 0.02715 + 0.03136 + 0.002;
            holokitModel.LensToEye = 0.02497 + 0.03898;
            holokitModel.AxisToBottom = 0.02990;
            holokitModel.HorizontalAlignmentMarkerOffset = 0.05075;
            break;
        default:
            break;
    }
    return holokitModel;
}

+ (PhoneType)getPhoneType {
    struct utsname systemInfo;
    uname(&systemInfo);
    NSString *deviceName = [NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding];
    if ([deviceName isEqualToString:@"iPhone11,2"]) {
        return iPhoneXS;
    } else if ([deviceName isEqualToString:@"iPhone11,4"] || [deviceName isEqualToString:@"iPhone11,6"]) {
        return iPhoneXSMax;
    } else if ([deviceName isEqualToString:@"iPhone12,3"]) {
        return iPhone11Pro;
    } else if ([deviceName isEqualToString:@"iPhone12,5"]) {
        return iPhone11ProMax;
    } else if ([deviceName isEqualToString:@"iPhone13,1"]) {
        return iPhone12mini;
    } else if ([deviceName isEqualToString:@"iPhone13,2"]) {
        return iPhone12;
    } else if ([deviceName isEqualToString:@"iPhone13,3"]) {
        return iPhone12Pro;
    } else if ([deviceName isEqualToString:@"iPhone13,4"]) {
        return iPhone12ProMax;
    } else if ([deviceName isEqualToString:@"iPhone14,4"]) {
        return iPhone13mini;
    } else if ([deviceName isEqualToString:@"iPhone14,5"]) {
        return iPhone13;
    } else if ([deviceName isEqualToString:@"iPhone14,2"]) {
        return iPhone13Pro;
    } else if ([deviceName isEqualToString:@"iPhone14,3"]) {
        return iPhone13ProMax;
    } else if ([deviceName isEqualToString:@"iPhone14,7"]) {
        return iPhone14;
    } else if ([deviceName isEqualToString:@"iPhone14,8"]) {
        return iPhone14Plus;
    } else if ([deviceName isEqualToString:@"iPhone15,2"]) {
        return iPhone14Pro;
    } else if ([deviceName isEqualToString:@"iPhone15,3"]) {
        return iPhone14ProMax;
    } else if ([deviceName isEqualToString:@"iPhone15,4"]) {
        return iPhone15;
    } else if ([deviceName isEqualToString:@"iPhone15,5"]) {
        return iPhone15Plus;
    } else if ([deviceName isEqualToString:@"iPhone16,1"]) {
        return iPhone15Pro;
    } else if ([deviceName isEqualToString:@"iPhone16,2"]) {
        return iPhone15ProMax;
    }else if ( [deviceName isEqualToString:@"iPad13,4"] || //iPad Pro 11 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,5"] || //iPad Pro 11 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,6"] || //iPad Pro 11 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,7"] || //iPad Pro 11 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,8"] || //iPad Pro 12.9 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,9"] || //iPad Pro 12.9 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,10"] || //iPad Pro 12.9 inch 5th Gen
               [deviceName isEqualToString:@"iPad13,11"] || //iPad Pro 12.9 inch 5th Gen
               [deviceName isEqualToString:@"iPad14,3"] || //iPad Pro 11 inch 4th Gen
               [deviceName isEqualToString:@"iPad14,4"] ||  //iPad Pro 11 inch 4th Gen
               [deviceName isEqualToString:@"iPad14,5"] || //iPad Pro 12.9 inch 6th Gen
               [deviceName isEqualToString:@"iPad14,6"]  //iPad Pro 12.9 inch 6th Gen
              ) {
        return iPadWithLidar;
    } else {
        return Unknown;
    }
}

+ (PhoneModel)getPhoneModel {
    PhoneModel phoneModel;
    switch ([HoloKitProfile getPhoneType])
    {
        case iPhoneXS:
            phoneModel.ScreenWidth = 0.135097;
            phoneModel.ScreenHeight = 0.062391;
            phoneModel.ScreenBottom = 0.00391;
            phoneModel.CameraOffset = simd_make_float3(0.05986, -0.055215, -0.0091);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhoneXSMax:
            phoneModel.ScreenWidth = 0.14971;
            phoneModel.ScreenHeight = 0.06961;
            phoneModel.ScreenBottom = 0.00391;
            phoneModel.CameraOffset = simd_make_float3(0.06694, -0.09405, -0.00591);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone11Pro:
            phoneModel.ScreenWidth = 0.13495;
            phoneModel.ScreenHeight = 0.06233;
            phoneModel.ScreenBottom = 0.00452;
            phoneModel.CameraOffset = simd_make_float3(0.059955, -0.05932, -0.00591);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone11ProMax:
            phoneModel.ScreenWidth = 0.14891;
            phoneModel.ScreenHeight = 0.06881;
            phoneModel.ScreenBottom = 0.00452;
            phoneModel.CameraOffset = simd_make_float3(0.066935, -0.0658, -0.00591);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone12mini:
            phoneModel.ScreenWidth = 0.12496;
            phoneModel.ScreenHeight = 0.05767;
            phoneModel.ScreenBottom = 0.00327;
            phoneModel.CameraOffset = simd_make_float3(0.05508, -0.05354, -0.00620);
            phoneModel.ScreenDpi = 476;
            break;
        case iPhone12:
            phoneModel.ScreenWidth = 0.13977;
            phoneModel.ScreenHeight = 0.06458;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.060625, -0.05879, -0.00633);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone12Pro:
            phoneModel.ScreenWidth = 0.13977;
            phoneModel.ScreenHeight = 0.06458;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.061195, -0.05936, -0.00551);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone12ProMax:
            phoneModel.ScreenWidth = 0.15390;
            phoneModel.ScreenHeight = 0.07113;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.04952, -0.06464, -0.00591);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone13mini:
            phoneModel.ScreenWidth = 0.12496;
            phoneModel.ScreenHeight = 0.05767;
            phoneModel.ScreenBottom = 0.00327;
            phoneModel.CameraOffset = simd_make_float3(0.0549, -0.05336, -0.00633);
            phoneModel.ScreenDpi = 476;
            break;
        case iPhone13:
            phoneModel.ScreenWidth = 0.13977;
            phoneModel.ScreenHeight = 0.06458;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.06147, -0.05964, -0.00781);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone13Pro:
            phoneModel.ScreenWidth = 0.13977;
            phoneModel.ScreenHeight = 0.06458;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.042005, -0.05809, -0.00727);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone13ProMax:
            phoneModel.ScreenWidth = 0.15390;
            phoneModel.ScreenHeight = 0.07113;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.04907, -0.06464, -0.00727);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone14:
            phoneModel.ScreenWidth = 0.13977;
            phoneModel.ScreenHeight = 0.06458;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.061475, -0.05964, -0.00848);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone14Plus:
            phoneModel.ScreenWidth = 0.15390;
            phoneModel.ScreenHeight = 0.07113;
            phoneModel.ScreenBottom = 0.00347;
            phoneModel.CameraOffset = simd_make_float3(0.06787, -0.06552, -0.00851);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone14Pro:
            phoneModel.ScreenWidth = 0.14109;
            phoneModel.ScreenHeight = 0.06508;
            phoneModel.ScreenBottom = 0.003185;
            phoneModel.CameraOffset = simd_make_float3(0.04021, -0.05717, -0.00784);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone14ProMax:
            phoneModel.ScreenWidth = 0.15434;
            phoneModel.ScreenHeight = 0.07121;
            phoneModel.ScreenBottom = 0.003185;
            phoneModel.CameraOffset = simd_make_float3(0.046835, -0.0633, -0.0078);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone15:
            phoneModel.ScreenWidth = 0.141136;
            phoneModel.ScreenHeight = 0.0651013;
            phoneModel.ScreenBottom = 0.003275;
            phoneModel.CameraOffset = simd_make_float3(0.04818, -0.042715, -0.0069);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone15Plus:
            phoneModel.ScreenWidth = 0.154388;
            phoneModel.ScreenHeight = 0.0712304;
            phoneModel.ScreenBottom = 0.003275;
            phoneModel.CameraOffset = simd_make_float3(0.054805, -0.048845, -0.0069);
            phoneModel.ScreenDpi = 458;
            break;
        case iPhone15Pro:
            phoneModel.ScreenWidth = 0.141136;
            phoneModel.ScreenHeight = 0.0651013;
            phoneModel.ScreenBottom = 0.00276;
            phoneModel.CameraOffset = simd_make_float3(0.039895, -0.03254, -0.00759);
            phoneModel.ScreenDpi = 460;
            break;
        case iPhone15ProMax:
            phoneModel.ScreenWidth = 0.154388;
            phoneModel.ScreenHeight = 0.0712304;
            phoneModel.ScreenBottom = 0.00276;
            phoneModel.CameraOffset = simd_make_float3(0.04652, -0.0598, -0.00773);
            phoneModel.ScreenDpi = 460;
            break;
        case iPadWithLidar:
        case Unknown:
        default:
            phoneModel.ScreenWidth = 0.15390;
            phoneModel.ScreenHeight = 0.07113;
            phoneModel.ScreenBottom = 0.00276;
            phoneModel.CameraOffset = simd_make_float3(0.04652, -0.0598, -0.00773);
            phoneModel.ScreenDpi = 460;
            break;
    }
    return phoneModel;
}

+ (BOOL)IsCurrentDeviceSupportedByHoloKit {
    PhoneType phoneType = [HoloKitProfile getPhoneType];
    if (phoneType == iPhone12mini) {
        return false;
    }
    if (phoneType == iPhone13mini) {
        return false;
    }
    if (phoneType == iPadWithLidar) {
        return false;
    }
    if (phoneType == Unknown) {
        return false;
    }
    return true;
}

+ (BOOL)IsCurrentDeviceIpad {
    PhoneType phoneType = [HoloKitProfile getPhoneType];
    return phoneType == iPadWithLidar;
}

+ (BOOL)IsCurrentDeviceEquippedWithLiDAR {
    PhoneType phoneType = [HoloKitProfile getPhoneType];
    if (phoneType == iPhone12Pro) {
        return true;
    }
    if (phoneType == iPhone12ProMax) {
        return true;
    }
    if (phoneType == iPhone13Pro) {
        return true;
    }
    if (phoneType == iPhone13ProMax) {
        return true;
    }
    if (phoneType == iPhone14Pro) {
        return true;
    }
    if (phoneType == iPhone14ProMax) {
        return true;
    }
    if (phoneType == iPhone15Pro) {
        return true;
    }
    if (phoneType == iPhone15ProMax) {
        return true;
    }
    if (phoneType == iPadWithLidar) {
        return true;
    }

    return false;
}

@end
