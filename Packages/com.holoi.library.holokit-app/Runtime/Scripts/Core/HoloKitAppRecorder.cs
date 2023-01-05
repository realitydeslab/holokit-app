using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using NatML.Recorders;
using NatML.Recorders.Clocks;
using NatML.Recorders.Inputs;
using NatML.Devices;
using NatSuite.Sharing;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public class HoloKitAppRecorder : MonoBehaviour
    {
        [Header(@"Recording")]
        [SerializeField] private int _frameRate = 30;

        [SerializeField] private Texture _watermarkImage;

        public bool IsRecording => _cameraInput != null;

        private bool _recordMicrophone;

        private bool _watermarkEnabled;

        // To be adjusted
        private int _videoWidth = 1170;

        // To be adjusted
        private int _videoHeight = 2532;

        // The screen AR camera
        private Camera _recordCamera;

        // Or MP4?
        private HEVCRecorder _recorder;

        // Camera input
        private CameraInput _cameraInput;

        // Watermark input
        //private WatermarkTextureInput _watermarkInput;

        // Audio input
        private AudioInput _audioInput;

        private AudioDevice _audioDevice;

        private AudioListener _cameraAudioListener;

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
            if (HoloKitUtils.IsRuntime)
            {
                _recordCamera = HoloKitCamera.Instance.GetComponent<Camera>();

                if (HoloKitApp.Instance.GlobalSettings.RecordMicrophone)
                {
                    _recordMicrophone = true;
                    // Initialize NatDevice
                    var query = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);
                    _audioDevice = query.current as AudioDevice;
                }
                else
                {
                    _recordMicrophone = false;
                    _cameraAudioListener = HoloKitCamera.Instance.GetComponent<AudioListener>();
                }

                _watermarkEnabled = HoloKitApp.Instance.GlobalSettings.WatermarkEnabled;
            }
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void Update()
        {
            // TODO: Dynamically adjust watermark based on device orientation
        }

        private void OnDestroy()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
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
            if (HoloKitUtils.IsEditor)
            {
                Debug.Log("[HoloKitAppRecorder] Cannot record in editor mode");
                return;
            }

            StartRecordingInternal();
        }

        private void CalculateVideoResolution()
        {
            // Set video width and height, notice that they can only be even numbers
            _videoWidth = Screen.width;
            _videoHeight = Screen.height;
            if (_videoWidth % 2 != 0)
                _videoWidth--;
            if (_videoHeight % 2 != 0)
                _videoHeight--;
        }

        private void StartRecordingInternal()
        {
            CalculateVideoResolution();
            // Open the screen AR camera if it is not already open
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = true;
                _recordCamera.enabled = true;
            }

            // Start recording
            var sampleRate = _recordMicrophone ? _audioDevice.sampleRate : 24000;
            var channelCount = _recordMicrophone ? _audioDevice.channelCount : 2;
            var clock = new RealtimeClock();
            _recorder = new HEVCRecorder(_videoWidth, _videoHeight, _frameRate, sampleRate, channelCount);

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
            _cameraInput = new(_recorder, clock, _recordCamera);

            _cameraInput.HDR = true;
            if (_recordMicrophone)
            {
                _audioDevice.StartRunning(audioBuffer =>
                {
                    _recorder.CommitSamples(audioBuffer.sampleBuffer, clock.timestamp);
                });
            }
            else
            {
                _audioInput = new AudioInput(_recorder, clock, _cameraAudioListener);
            }
            OnRecordingStarted?.Invoke();
        }

        public async void StopRecording()
        {
            if (HoloKitUtils.IsEditor)
            {
                Debug.Log("[HoloKitAppRecorder] Cannot record in editor mode");
                return;
            }

            //_watermarkInput?.Dispose();
            _cameraInput?.Dispose();
            if (_recordMicrophone)
            {
                _audioDevice.StopRunning();
            }
            else
            {
                _audioInput?.Dispose();
            }
            var path = await _recorder.FinishWriting();

            // Save to Photo Library
            var payload = new SavePayload();
            payload.AddMedia(path);
            //await payload.Save();
            await payload.Commit();

            // Close the screen AR camera if necessary
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = false;
                _recordCamera.enabled = false;
            }

            // Release inputs
            //_watermarkInput = null;
            _cameraInput = null;
            _audioInput = null;

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
