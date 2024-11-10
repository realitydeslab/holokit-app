// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

#import <AVFoundation/AVFoundation.h>
#import <Photos/Photos.h>
#import <UIKit/UIKit.h>
#import <CoreLocation/CoreLocation.h>
#import <UserNotifications/UserNotifications.h>
#include <Availability.h>

typedef enum {
    CameraPermissionStatusNotDetermined = 0,
    CameraPermissionStatusRestricted = 1,
    CameraPermissionStatusDenied = 2,
    CameraPermissionStatusGranted = 3,
} CameraPermissionStatus;

typedef enum {
    MicrophonePermissionStatusNotDetermined = 0,
    MicrophonePermissionStatusDenied = 1,
    MicrophonePermissionStatusGranted = 2
} MicrophonePermissionStatus;

typedef enum {
    PhotoLibraryPermissionStatusNotDetermined = 0,
    PhotoLibraryPermissionStatusRestricted = 1,
    PhotoLibraryPermissionStatusDenied = 2,
    PhotoLibraryPermissionStatusGranted = 3,
    PhotoLibraryPermissionStatusLimited = 4
} PhotoLibraryPermissionStatus;

typedef enum {
    LocationPermissionStatusNotDetermined = 0,
    LocationPermissionStatusRestricted = 1,
    LocationPermissionStatusDenied = 2,
    LocationPermissionStatusAuthorizedAlways = 3,
    LocationPermissionStatusAuthorizedWhenInUse = 4,
    LocationPermissionStatusAuthorized = 5
} LocationPermissionStatus;

void (*OnRequestCameraPermissionCompleted)(bool) = NULL;
void (*OnRequestMicrophonePermissionCompleted)(bool) = NULL;
void (*OnRequestPhotoLibraryAddPermissionCompleted)(int) = NULL;
void (*OnLocationPermissionStatusChanged)(int) = NULL;

@interface Permissions : NSObject

@end

@interface Permissions() <CLLocationManagerDelegate>

@property (nonatomic, strong) CLLocationManager *locationManager;

@end

@implementation Permissions

- (instancetype)init {
    self = [super init];
    if (self) {
        self.locationManager = [[CLLocationManager alloc] init];
        self.locationManager.delegate = self;
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

+ (void)openAppSettings {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString: UIApplicationOpenSettingsURLString] options:@{} completionHandler:nil];
}

+ (CameraPermissionStatus)getCameraPermissionStatus {
    AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    return (CameraPermissionStatus)status;
}

+ (void)requestCameraPermission {
    [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
        if (OnRequestCameraPermissionCompleted != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnRequestCameraPermissionCompleted(granted);
            });
        }
    }];
}

+ (MicrophonePermissionStatus)getMicrophonePermissionStatus {
    if (@available(iOS 17.0, *)) {
        AVAudioApplicationRecordPermission status = [[AVAudioApplication sharedInstance] recordPermission];
        MicrophonePermissionStatus microphoneStatus = 0;
        switch (status) {
            case AVAudioApplicationRecordPermissionUndetermined:
                microphoneStatus = MicrophonePermissionStatusNotDetermined;
                break;
            case AVAudioApplicationRecordPermissionGranted:
                microphoneStatus = MicrophonePermissionStatusGranted;
                break;
            case AVAudioApplicationRecordPermissionDenied:
                microphoneStatus = MicrophonePermissionStatusDenied;
                break;
            default:
                break;
        }
        return (MicrophonePermissionStatus) microphoneStatus;
    } 
    #if TARGET_OS_IPHONE && __IPHONE_OS_VERSION_MIN_REQUIRED && __IPHONE_OS_VERSION_MIN_REQUIRED < 170000 || TARGET_OS_OSX && __MAC_OS_X_VERSION_MIN_REQUIRED && __MAC_OS_X_VERSION_MIN_REQUIRED < 140000
    else {
        AVAudioSessionRecordPermission status = [[AVAudioSession sharedInstance] recordPermission];
        NSLog(@"NSLog AVAudioSessionRecordPermission %ld", (long)((MicrophonePermissionStatus)status));
        MicrophonePermissionStatus microphoneStatus = 0;
        switch (status) {
            case AVAudioSessionRecordPermissionUndetermined:
                microphoneStatus = MicrophonePermissionStatusNotDetermined;
                break;
            case AVAudioSessionRecordPermissionGranted:
                microphoneStatus = MicrophonePermissionStatusGranted;
                break;
            case AVAudioSessionRecordPermissionDenied:
                microphoneStatus = MicrophonePermissionStatusDenied;
                break;
            default:
                break;
        }
        return (MicrophonePermissionStatus) microphoneStatus;
    }
    #endif
}

+ (void)requestMicrophonePermission {
    if (@available(iOS 17.0, *)) {
        [AVAudioApplication requestRecordPermissionWithCompletionHandler: ^(BOOL granted) {
            if (OnRequestMicrophonePermissionCompleted != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnRequestMicrophonePermissionCompleted(granted);
                });
            }
        }];
    } 
    #if TARGET_OS_IPHONE && __IPHONE_OS_VERSION_MIN_REQUIRED && __IPHONE_OS_VERSION_MIN_REQUIRED < 170000 || TARGET_OS_OSX && __MAC_OS_X_VERSION_MIN_REQUIRED && __MAC_OS_X_VERSION_MIN_REQUIRED < 140000
    else {
        [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) {
            if (OnRequestMicrophonePermissionCompleted != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnRequestMicrophonePermissionCompleted(granted);
                });
            }
        }];
    }
    #endif
}

+ (PhotoLibraryPermissionStatus)getPhotoLibraryAddPermissionStatus {
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatusForAccessLevel:PHAccessLevelAddOnly];
    return (PhotoLibraryPermissionStatus)status;
}

+ (void)requestPhotoLibraryAddPermission {
    [PHPhotoLibrary requestAuthorizationForAccessLevel:PHAccessLevelAddOnly handler:^(PHAuthorizationStatus status) {
        if (OnRequestPhotoLibraryAddPermissionCompleted != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnRequestPhotoLibraryAddPermissionCompleted((int)status);
            });
        }
    }];
}

- (LocationPermissionStatus)getLocationPermissionStatus {
    CLAuthorizationStatus status = [self.locationManager authorizationStatus];
    return (LocationPermissionStatus)status;
}

- (void)requestLocationWhenInUsePermission {
    [self.locationManager requestWhenInUseAuthorization];
}

- (void)requestLocationAlwaysPermission {
    [self.locationManager requestAlwaysAuthorization];
}

#pragma mark - CLLocationManagerDelegate

- (void)locationManagerDidChangeAuthorization:(CLLocationManager *)manager {
    if (OnLocationPermissionStatusChanged != NULL) {
        CLAuthorizationStatus status = [self.locationManager authorizationStatus];
        dispatch_async(dispatch_get_main_queue(), ^{
            OnLocationPermissionStatusChanged((int)status);
        });
    }
}

#pragma mark - Marshalling

void Permissions_Initialize(void (*OnRequestCameraPermissionCompletedDelegate)(bool),
                            void (*OnRequestMicrophonePermissionCompletedDelegate)(bool),
                            void (*OnRequestPhotoLibraryAddPermissionCompletedDelegate)(int),
                            void (*OnLocationPermissionStatusChangedDelegate)(int)) {
    OnRequestCameraPermissionCompleted = OnRequestCameraPermissionCompletedDelegate;
    OnRequestMicrophonePermissionCompleted = OnRequestMicrophonePermissionCompletedDelegate;
    OnRequestPhotoLibraryAddPermissionCompleted = OnRequestPhotoLibraryAddPermissionCompletedDelegate;
    OnLocationPermissionStatusChanged = OnLocationPermissionStatusChangedDelegate;
}

int Permissions_GetCameraPermissionStatus(void) {
    return (int)[Permissions getCameraPermissionStatus];
}

void Permissions_RequestCameraPermission(void) {
    [Permissions requestCameraPermission];
}

int Permissions_GetMicrophonePermissionStatus(void) {
    return (int)[Permissions getMicrophonePermissionStatus];
}

void Permissions_RequestMicrophonePermission(void) {
    [Permissions requestMicrophonePermission];
}

int Permissions_GetPhotoLibraryAddPermissionStatus(void) {
    return (int)[Permissions getPhotoLibraryAddPermissionStatus];
}

void Permissions_RequestPhotoLibraryAddPermission(void) {
    [Permissions requestPhotoLibraryAddPermission];
}

int Permissions_GetLocationPermissionStatus(void) {
    return (int)[[Permissions sharedInstance] getLocationPermissionStatus];
}

void Permissions_RequestLocationWhenInUsePermission(void) {
    [[Permissions sharedInstance] requestLocationWhenInUsePermission];
}

void Permissions_RequestLocationAlwaysPermission(void) {
    [[Permissions sharedInstance] requestLocationAlwaysPermission];
}

void Permissions_OpenAppSettings(void) {
    [Permissions openAppSettings];
}

@end
