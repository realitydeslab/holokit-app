// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib
{
    [DisallowMultipleComponent]
    public class HoloKitAppRecorder : MonoBehaviour
    {
        [Header(@"Recording")]
        [SerializeField] private int _frameRate = 30;

        [SerializeField] private Texture _watermarkImage;

        public bool IsRecording => _recorder.IsRecording;

        // Or MP4?
        private HoloKitVideoRecorder _recorder;

        // Camera input
        //private CameraInput _cameraInput;

        // Watermark input
        //private WatermarkTextureInput _watermarkInput;

        // Audio input
        //private AudioInput _audioInput;

        //private AudioDevice _audioDevice;

        //private AudioListener _cameraAudioListener;

        private const float WatermarkPortraitPosXRatio = 0.0615f;

        private const float WatermarkPortraitPosYRatio = 0.0284f;

        private const float WatermarkPortraitWidthRatio = 0.2949f;

        private const float WatermarkPortraitHeightRatio = 0.0284f;

        private const float WatermarkLandscapePosXRatio = 0.0284f;

        private const float WatermarkLandscapePosYRatio = 0.0615f;

        private const float WatermarkLandscapeWidthRatio = 0.1363f;

        private const float WatermarkLandscapeHeightRatio = 0.0615f;

        public static event Action OnRecordingStarted;

        public static event Action OnRecordingStopped;

        private void Start()
        {
            _recorder = HoloKitCameraManager.Instance.GetComponent<HoloKitVideoRecorder>();

            if (HoloKitUtils.IsRuntime)
            {
                if (HoloKitApp.Instance.GlobalSettings.RecordMicrophone)
                {
                   // _recorder.recordMicrophone = true;

                }
                else
                {
                  //  _recorder.recordMicrophone = false;
                }

                //_watermarkEnabled = HoloKitApp.Instance.GlobalSettings.WatermarkEnabled;
            }
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void Update()
        {
            // TODO: Dynamically adjust watermark based on device orientation
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        public void ToggleRecording()
        {
            if (IsRecording)
                StopRecording();
            else
                StartRecording();
        }

        public void StartRecording()
        {
            if (_recorder) {
                _recorder.StartRecording();
            }
        }

        private void StartRecordingInternal()
        {
            //CalculateVideoResolution();
            // Open the screen AR camera if it is not already open
            //if (HoloKitCameraManager.Instance.RenderMode == HoloKitRenderMode.Stereo)
            //{
            //    _recordCamera.GetComponent<ARCameraBackground>().enabled = true;
            //    _recordCamera.enabled = true;
            //}

            // Start recording
            //var sampleRate = _recordMicrophone ? _audioDevice.sampleRate : 24000;
            //var channelCount = _recordMicrophone ? _audioDevice.channelCount : 2;
            //var clock = new RealtimeClock();
            //_recorder = new HoloKitVideoRecorder(_videoWidth, _videoHeight, _frameRate, sampleRate, channelCount);

            //if (_watermarkEnabled)
            //{
            //    _watermarkInput = new WatermarkTextureInput(_recorder);
            //    _watermarkInput.watermark = _watermarkImage;
            //    if (Screen.orientation == ScreenOrientation.Portrait)
            //    {
            //        _watermarkInput.rect = new(Mathf.RoundToInt(_videoWidth * WatermarkPortraitPosXRatio),
            //                                   Mathf.RoundToInt(_videoHeight * WatermarkPortraitPosYRatio),
            //                                   Mathf.RoundToInt(_videoWidth * WatermarkPortraitWidthRatio),
            //                                   Mathf.RoundToInt(_videoHeight * WatermarkPortraitHeightRatio));
            //    }
            //    else if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            //    {
            //        _watermarkInput.rect = new(Mathf.RoundToInt(_videoWidth * WatermarkLandscapePosXRatio),
            //                                   Mathf.RoundToInt(_videoHeight * WatermarkLandscapePosYRatio),
            //                                   Mathf.RoundToInt(_videoWidth * WatermarkLandscapeWidthRatio),
            //                                   Mathf.RoundToInt(_videoHeight * WatermarkLandscapeHeightRatio));
            //    }
            //    _cameraInput = new(_watermarkInput, clock, _recordCamera);
            //}
            //else
            //{
            //    _watermarkInput = null;
            //    _cameraInput = new(_recorder, clock, _recordCamera);
            //}

            // This line is the temporary replacement for the above commented section since watermark is now not supported
            // _cameraInput = new(_recorder, clock, _recordCamera);

            //if (_recordMicrophone)
            //{
            //    // _audioDevice.StartRunning(audioBuffer =>
            //    // {
            //    //     _recorder.CommitSamples(audioBuffer.sampleBuffer, clock.timestamp);
            //    // });
            //}
            //else
            //{
            //   // _audioInput = new AudioInput(_recorder, clock, _cameraAudioListener);
            //}

            //_recorder
            OnRecordingStarted?.Invoke();
        }

        public void StopRecording()
        {
            //if (HoloKitUtils.IsEditor)
            //{
            //    Debug.Log("[HoloKitAppRecorder] Cannot record in editor mode");
            //    return;
            //}

            //_watermarkInput?.Dispose();
            //_cameraInput?.Dispose();
            //if (_recordMicrophone)
            //{
            //    _audioDevice.StopRunning();
            //}
            //else
            //{
            //    _audioInput?.Dispose();
            //}
            //var path = await _recorder.FinishWriting();

            //// Save to Photo Library
            //var payload = new SavePayload();
            //payload.AddMedia(path);
            ////await payload.Save();
            //await payload.Commit();

            //// Close the screen AR camera if necessary
            //if (HoloKitCameraManager.Instance.RenderMode == HoloKitRenderMode.Stereo)
            //{
            //    _recordCamera.GetComponent<ARCameraBackground>().enabled = false;
            //    _recordCamera.enabled = false;
            //}

            //// Release inputs
            ////_watermarkInput = null;
            //_cameraInput = null;
            //_audioInput = null;

            if (_recorder)
                _recorder.EndRecording();

            OnRecordingStopped?.Invoke();
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (IsRecording)
            {
                StopRecording();
                Debug.Log("[Recorder] Recording interrupted due to a render mode change");
            }
        }
    }
}
