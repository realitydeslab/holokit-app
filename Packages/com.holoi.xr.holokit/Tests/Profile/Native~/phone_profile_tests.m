// SPDX-FileCopyrightText: Copyright 2026 Reality Design Lab <dev@reality.design>
// SPDX-License-Identifier: MIT

// Host tests for the phone table compiled into libUnityHoloKit.a (Sources~/holokit/HoloKitProfile.m).
// The script compiles HoloKitProfile.m with uname renamed to FakeUname, so each case pretends to be one phone.

#import <Foundation/Foundation.h>
#import <sys/utsname.h>
#import <math.h>
#import <string.h>
#import "HoloKitProfile.h"

int FakeUname(struct utsname *name);

static const char *currentMachine = "";
static int failures = 0;

int FakeUname(struct utsname *name) {
    memset(name, 0, sizeof(*name));
    strlcpy(name->machine, currentMachine, sizeof(name->machine));
    return 0;
}

typedef struct {
    const char *machine;
    const char *phone;
    BOOL supported;
    BOOL lidar;
    float screenWidth;
    float screenHeight;
    float screenBottom;
    simd_float3 cameraOffset;
} PhoneCase;

static void Expect(BOOL ok, const PhoneCase *c, const char *what) {
    if (ok) return;
    failures++;
    fprintf(stderr, "FAIL %s (%s): %s\n", c->phone, c->machine, what);
}

static BOOL Near(float a, float b) {
    return fabsf(a - b) < 1e-6f;
}

static void Check(const PhoneCase *c) {
    currentMachine = c->machine;
    Expect([HoloKitProfile IsCurrentDeviceSupportedByHoloKit] == c->supported, c, "supported by HoloKit");
    Expect([HoloKitProfile IsCurrentDeviceEquippedWithLiDAR] == c->lidar, c, "LiDAR");
    Expect(![HoloKitProfile IsCurrentDeviceIpad], c, "not an iPad");
    if (!c->supported) return;
    PhoneModel model = [HoloKitProfile getPhoneModel];
    Expect(Near(model.ScreenWidth, c->screenWidth), c, "screen width");
    Expect(Near(model.ScreenHeight, c->screenHeight), c, "screen height");
    Expect(Near(model.ScreenBottom, c->screenBottom), c, "screen bottom");
    Expect(Near(model.CameraOffset.x, c->cameraOffset.x) && Near(model.CameraOffset.y, c->cameraOffset.y)
           && Near(model.CameraOffset.z, c->cameraOffset.z), c, "camera offset");
    Expect(Near(model.ScreenDpi, 460), c, "screen dpi");
}

typedef struct {
    const char *machine;
    const char *name;
    float portraitPixelsWide;  // Apple's tech specs; every iPad Pro with LiDAR has 264 pixels per inch
    float portraitPixelsHigh;
} IpadCase;

static BOOL NearMm(float a, float b) {
    return fabsf(a - b) < 1e-5f;
}

// iPads with LiDAR do not fit the HoloKit headset, but run the LiDAR realities on the screen and host shared realities:
// the QR code a spectator scans is sized and placed with the screen's real size, dpi and camera position.
static void CheckIpadWithLidar(const IpadCase *ipad) {
    const PhoneCase c = {ipad->machine, ipad->name, NO, YES, 0, 0, 0, simd_make_float3(0, 0, 0)};
    currentMachine = ipad->machine;
    Expect(![HoloKitProfile IsCurrentDeviceSupportedByHoloKit], &c, "not supported by HoloKit");
    Expect([HoloKitProfile IsCurrentDeviceEquippedWithLiDAR], &c, "LiDAR");
    Expect([HoloKitProfile IsCurrentDeviceIpad], &c, "an iPad");
    PhoneModel model = [HoloKitProfile getPhoneModel];
    const float wide = ipad->portraitPixelsWide / 264.0f * 0.0254f, high = ipad->portraitPixelsHigh / 264.0f * 0.0254f;
    Expect(Near(model.ScreenDpi, 264), &c, "screen dpi 264");
    Expect(NearMm(model.ScreenWidth, high) && NearMm(model.ScreenHeight, wide), &c, "screen size (landscape)");
    // Camera to the middle of the screen's left edge, as (-y, x) in portrait: the rear camera sits behind the
    // display's top-right corner seen from the front, 6.5 mm in from both edges (not measured).
    Expect(NearMm(model.CameraOffset.x, high / 2 - 0.0065f) && NearMm(model.CameraOffset.y, -wide + 0.0065f)
           && NearMm(model.CameraOffset.z, -0.0045f), &c, "camera offset");
}

int main(void) {
    @autoreleasepool {
        const simd_float3 proCamera = simd_make_float3(0.039895, -0.03254, -0.00759);
        const simd_float3 proMaxCamera = simd_make_float3(0.04652, -0.0598, -0.00773);
        const simd_float3 baseCamera = simd_make_float3(0.04818, -0.042715, -0.0069);
        const simd_float3 plusCamera = simd_make_float3(0.054805, -0.048845, -0.0069);
        const PhoneCase cases[] = {
            // Already in the table before iPhone 17: unchanged.
            {"iPhone16,1", "iPhone 15 Pro", YES, YES, 0.141136f, 0.0651013f, 0.00276f, proCamera},
            {"iPhone17,3", "iPhone 16", YES, NO, 0.14109f, 0.06508f, 0.00327f, baseCamera},
            {"iPhone17,4", "iPhone 16 Plus", YES, NO, 0.15434f, 0.07121f, 0.00327f, plusCamera},
            {"iPhone17,1", "iPhone 16 Pro", YES, YES, 0.14479f, 0.06657f, 0.00244f, proCamera},
            {"iPhone17,2", "iPhone 16 Pro Max", YES, YES, 0.15831f, 0.07286f, 0.00236f, proMaxCamera},
            // iPhone 17 and 18 (model ids from Xcode 27.1's device types).
            {"iPhone18,3", "iPhone 17", YES, NO, 0.14479f, 0.06657f, 0.00327f, baseCamera},
            {"iPhone18,4", "iPhone Air", YES, NO, 0.15107f, 0.06957f, 0.00327f, plusCamera},
            {"iPhone18,1", "iPhone 17 Pro", YES, YES, 0.14479f, 0.06657f, 0.00244f, proCamera},
            {"iPhone18,2", "iPhone 17 Pro Max", YES, YES, 0.15831f, 0.07286f, 0.00236f, proMaxCamera},
            {"iPhone19,2", "iPhone 18 Pro", YES, YES, 0.14479f, 0.06657f, 0.00244f, proCamera},
            {"iPhone19,3", "iPhone 18 Pro Max", YES, YES, 0.15831f, 0.07286f, 0.00236f, proMaxCamera},
            // Not supported: no values for these phones.
            {"iPhone17,5", "iPhone 16e", NO, NO, 0, 0, 0, proCamera},
            {"iPhone18,5", "iPhone 17e", NO, NO, 0, 0, 0, proCamera},
            {"iPhone19,4", "iPhone Duo", NO, NO, 0, 0, 0, proCamera},
            {"iPhone99,9", "a future iPhone", NO, NO, 0, 0, 0, proCamera},
        };
        const size_t count = sizeof(cases) / sizeof(cases[0]);
        for (size_t i = 0; i < count; i++) {
            Check(&cases[i]);
        }
        // Every iPad Pro with LiDAR, Wi-Fi and cellular (Xcode 27.1's device types list 8,9 8,12 13,5 13,10 14,3 14,5
        // 16,4 16,6 17,2 17,4; the others are their pairs).
        const IpadCase ipads[] = {
            {"iPad8,9", "iPad Pro 11-inch 2nd gen", 1668, 2388}, {"iPad8,10", "iPad Pro 11-inch 2nd gen", 1668, 2388},
            {"iPad8,11", "iPad Pro 12.9-inch 4th gen", 2048, 2732}, {"iPad8,12", "iPad Pro 12.9-inch 4th gen", 2048, 2732},
            {"iPad13,4", "iPad Pro 11-inch 3rd gen", 1668, 2388}, {"iPad13,7", "iPad Pro 11-inch 3rd gen", 1668, 2388},
            {"iPad13,8", "iPad Pro 12.9-inch 5th gen", 2048, 2732}, {"iPad13,11", "iPad Pro 12.9-inch 5th gen", 2048, 2732},
            {"iPad14,3", "iPad Pro 11-inch 4th gen", 1668, 2388}, {"iPad14,6", "iPad Pro 12.9-inch 6th gen", 2048, 2732},
            {"iPad16,3", "iPad Pro 11-inch (M4)", 1668, 2420}, {"iPad16,4", "iPad Pro 11-inch (M4)", 1668, 2420},
            {"iPad16,5", "iPad Pro 13-inch (M4)", 2064, 2752}, {"iPad16,6", "iPad Pro 13-inch (M4)", 2064, 2752},
            {"iPad17,1", "iPad Pro 11-inch (M5)", 1668, 2420}, {"iPad17,2", "iPad Pro 11-inch (M5)", 1668, 2420},
            {"iPad17,3", "iPad Pro 13-inch (M5)", 2064, 2752}, {"iPad17,4", "iPad Pro 13-inch (M5)", 2064, 2752},
        };
        const size_t ipadCount = sizeof(ipads) / sizeof(ipads[0]);
        for (size_t i = 0; i < ipadCount; i++) {
            CheckIpadWithLidar(&ipads[i]);
        }
        if (failures > 0) {
            fprintf(stderr, "%d failure(s) in %zu phones and %zu iPads\n", failures, count, ipadCount);
            return 1;
        }
        printf("All %zu phones and %zu iPads pass\n", count, ipadCount);
    }
    return 0;
}
