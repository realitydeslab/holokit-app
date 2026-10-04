# Low-latency tracking

In Stereo mode `LowLatencyTrackingManager` predicts the camera pose for the moment each
frame is seen, so world-locked content stays put while the head moves. It is added
automatically by `HoloKitTrackedPoseDriver` (on "Stereo Tracked Pose") unless its `Kalman Prediction`
switch is off, which falls back to the earlier Cardboard head tracker.

## Why prediction is needed

Measured on iPhone 15 Pro at 120 fps:

| Stage | Time |
| --- | --- |
| ARKit camera capture → ARFrame delivered to the app | 36–60 ms |
| ARFrame delivered → render start | 0–8 ms |
| Render start → `presentedTime` (frame on the display) | 17 ms |
| `presentedTime` → seen (panel scan-out, estimated) | ~4 ms |

Rendering the latest ARKit pose therefore shows where the head was 60–90 ms ago:
about 4° median error while turning, 10° above 120 °/s.

## How it works

The algorithm is the one in [holokit-swift-sdk](https://github.com/holokit/holokit-swift-sdk)
(`HoloKitKalmanPredictor`), ported line by line to C++:

1. **Kalman filter** (Cardboard-style sensor fusion with ARKit as the absolute
   measurement): an error-state EKF over orientation and gyro bias, and a Kalman filter
   over position and velocity. Core Motion device motion (100 Hz, ~1 ms delivery)
   drives it; every ARFrame corrects it at the frame's capture time.
2. **Prediction to the display time**: before each render the IMU samples received
   since the last ARFrame are integrated forward, and the last ~25 ms without sensor
   data are extrapolated with the measured angular acceleration. The target is not
   assumed: a render plugin delegate reads each frame's `CAMetalDrawable.presentedTime`,
   and the smoothed render-start-to-present time plus 4 ms scan-out sets the lead.
3. **Output smoothing**: the previous pose is carried forward with the IMU rate and
   only frame-to-frame jumps are low-passed (10 ms rotation, 30 ms position).

| File | Language | Role |
| --- | --- | --- |
| `Runtime/Plugins/iOS/LowLatencyTracking/HoloKitTrackingCore.h/.cpp` | C++ (`<simd/simd.h>`) | Kalman filter, smoother, Unity ↔ ARKit conversion. No ARKit or Unity, so it runs in host tests. |
| `Runtime/Plugins/iOS/LowLatencyTracking/HoloKitTracking.mm` | Objective-C++ | Core Motion, the session's newest ARFrame, `presentedTime` via `RenderPluginDelegate`, C API |
| `Runtime/LowLatencyTrackingManager.cs` | C# | Starts it in Stereo, corrects on `frameReceived`, applies the pose in `Application.onBeforeRender` |

The render delegate is chained with any delegate another plugin installed
(`RenderPluginArrayDelegate`).

## Accuracy

Orientation at the moment it is seen compared with where ARKit later reported the
camera (iPhone 15 Pro, wide head motion, ARKit 60 Hz; Swift SDK, same algorithm):

| | Median | p95 |
| --- | --- | --- |
| No prediction | 4.31° | 12.5° |
| Kalman prediction | **0.17°** | **0.80°** |
| Position, no prediction | 8.3 mm | 45.8 mm |
| Position, Kalman | **2.7 mm** | **9.1 mm** |

## Frame rate

While predicting, `LowLatencyTrackingManager` holds Stereo at 60 fps and turns off ARSession's Match Frame Rate
(which otherwise blocks each frame until the next ARKit frame, 30 Hz on a warm phone); Mono restores both. The
projects' "Thermal State Serious" frame rate is 60 instead of 30, so a warm phone keeps 60 fps; at "Critical" Unity
still drops to 15 fps.

## Tests

- `Tests/Tracking/Tools~/run_native_tests.sh`: builds the C++ core on the Mac and checks it against the Swift SDK
  on a recorded iPhone 15 Pro trace.
- `LowLatencyTrackingTests` (EditMode, HoloKit project): runs the host tests; the manager is inactive outside the
  iOS player.
- Device: `mofa-combat-stereo` requires the pose to be predicted in Stereo and records the measured present latency
  and lead time.

Ported from ../holokit-unity-sdk (Documentation~/LowLatencyTracking.md) on 2026-10-04.
