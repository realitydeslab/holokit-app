# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] - 2026-10-04

### Added

- Low-latency tracking from the HoloKit Unity SDK: in Stereo, `LowLatencyTrackingManager` predicts the camera pose
  for the moment each frame is seen (C++ Kalman filter over Core Motion, corrected by every ARKit frame, leading by
  the measured render-to-present time). `HoloKitTrackedPoseDriver` adds it to "Stereo Tracked Pose"; switch
  `Kalman Prediction` off to use the earlier Cardboard head tracker. Native code in
  `Runtime/Plugins/iOS/LowLatencyTracking`; host tests in `Tests/Tracking` (same results as the Swift SDK on an
  iPhone 15 Pro trace: rotation 0.21 deg median, 1.03 deg p95). See `Documentation~/LowLatencyTracking.md`.
- In Stereo the frame rate is held at 60 fps and ARSession's Match Frame Rate is off; both are restored in Mono.
- `ARKitCameraCompatibility` from the HoloKit Unity SDK (iOS 27): the AR camera takes ARKit's rotation, projection
  and background display transform for the current interface orientation. With ARKit XR 6.4.3 on iOS 27 AR
  Foundation's pose lacks the interface roll, so in portrait virtual content was 90 degrees off (measured 89.96 deg
  on an iPhone 15 Pro). Mono's Center Eye Pose is re-synced after the correction.

### Fixed

- Screen recordings now keep bloom: the recorder's camera texture is HDR (it was 8-bit, so colours clipped at 1).
- Screen recordings were 90 degrees off in portrait on iOS 27: the recorder rendered during Update, before the camera
  correction. It now records at the end of the frame, as the HoloKit Unity SDK does.

## [0.3.12] - 2023.10.28

### Added

- Added `SupportedMonoScreenOrientation` property in `HoloKitCameraManager` to reset the enabled auto rotate orientations after switching back to mono mode.

### Fixed

- Fixed stereo mono crashing due to Xcode 15 by adding `-ld64` to `Other Linker Flags` property in Xcode Build Settings of UnityFramework target.

## [0.3.11] - 2023.10.27

### Added

- Added AppleNativeProvider for getting iOS thermal state and SystemUptime.
- Added package dependency for ARKit.

### Changed

- Changed ARFoundation package dependency version from 5.0.6 to 5.1.0.

## [0.3.10] - 2023.10.26

### Added

- Added checking if the device supports LiDAR depth sensor.

## [0.3.9] - 2023.10.26

### Added

- Added support for iPhone 15, iPhone 15 Plus, iPhone 15 Pro and iPhone 15 Pro Max.
- Added gaze interaction components.
- Added gaze gesture interaction components.

## [0.3.8] - 2023.9.20

### Added

- Added `LowLatencyTrackingManager_3DoF` for 3DoF tracking.
- Added `3DoFTracking` sample scene.

### Removed

- Removed 3DoF tracking mode in `LowLatencyTrackingManager`. 

## [0.3.7] - 2023.9.7

### Changed

- Now the deprecated `ARPoseDriver` is also supported by `LowLatencyTrackingManager`.

## [0.3.6] - 2023.9.7

### Changed

- Script `HoloKitDefaultUICanvas` no longer locks the screen orientation.

## [0.3.5] - 2023.9.1

### Added

- Added 3DoF tracking mode for LowLatencyTrackingManager. This is only for internal use.
- Now when you create a HoloKitDefaultUICanvas, if there is no EventSystem in the scene, the SDK will create one for you.

## [0.3.4] - 2023.8.11

### Added

- Now you can add HoloKit DefaultUICanvas in the Unity Editor's Hierarchy Window

## [0.3.3] - 2023.8.10

### Added

- Now you can add HoloKit XROrigin in the Unity Editor's Hierarchy Window

## [0.3.2] - 2023.7.23

### Added

- HoloKit Unity SDK now supports Android devices
- Added PhoneModelSpecsCalibration sample
- Force screen brightness to 1.0 under Stereo mode on iOS

## [0.3.1] - 2023.7.4

### Added

- Added LowLatencyTrackingManager

### Changed

- Changed minimum compatible ARFoundation version to 5.+

### Removed

- Removed ARKit package dependency