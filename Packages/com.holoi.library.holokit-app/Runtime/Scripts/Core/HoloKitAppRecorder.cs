using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using NatML.Recorders;
using NatML.Recorders.Clocks;
using NatML.Recorders.Inputs;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
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
        private WatermarkTextureInput _watermarkInput;

        // Audio input
        private AudioInput _audioInput;

        private AudioSource _microphoneAudioSource;

        private AudioListener _cameraAudioListener;

        private const float WatermarkPortraitPosXRatio = 0.1487f;

        private const float WatermarkPortraitPosYRatio = 0.1066f;

        private const float WatermarkPortraitWidthRatio = 0.05f;

        private const float WatermarkPortraitHeightRatio = 0.0592f;

        private const float WatermarkLandscapePosXRatio = 0.0687f;

        private const float WatermarkLandscapePosYRatio = 0.1795f;

        private const float WatermarkLandscapeWidthRatio = 0.05f;

        private const float WatermarkLandscapeHeightRatio = 0.1282f;

        private void Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                _recordCamera = HoloKitCamera.Instance.GetComponent<Camera>();

                if (HoloKitApp.Instance.GlobalSettings.PhaseEnabled && HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
                {
                    _recordMicrophone = false;
                }
                else if (HoloKitApp.Instance.GlobalSettings.RecordMicrophone)
                {
                    _recordMicrophone = true;
                }

                _watermarkEnabled = HoloKitApp.Instance.GlobalSettings.WatermarkEnabled;
            }
        }

        public void StartRecording()
        {
            StartCoroutine(StartRecordingCoroutine());
        }

        private IEnumerator StartRecordingCoroutine()
        {
            // Set video width and height, notice that they can only be even numbers
            _videoWidth = Screen.width;
            _videoHeight = Screen.height;
            if (_videoWidth % 2 != 0)
            {
                _videoWidth--;
            }
            if (_videoHeight % 2 != 0)
            {
                _videoHeight--;
            }
            // Open the screen AR camera if it is not already open
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = true;
                _recordCamera.enabled = true;
            }

            if (_recordMicrophone)
            {
                _microphoneAudioSource = gameObject.AddComponent<AudioSource>();
                // TODO: What does 'mute' mean here?
                _microphoneAudioSource.mute = false;
                _microphoneAudioSource.loop = true;
                _microphoneAudioSource.bypassEffects = false;
                _microphoneAudioSource.bypassListenerEffects = false;
                _microphoneAudioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
                yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
                _microphoneAudioSource.Play();
            }
            else
            {
                _cameraAudioListener = HoloKitCamera.Instance.GetComponent<AudioListener>();
                yield return null;
            }

            // Start recording
            var sampleRate = _recordMicrophone ? AudioSettings.outputSampleRate : 24000;
            var channelCount = _recordMicrophone ? (int)AudioSettings.speakerMode : 2;
            //Debug.Log($"AudioSettings.outputSampleRate: {AudioSettings.outputSampleRate} and AudioSettings.speakerMode: {(int)AudioSettings.speakerMode}");
            var clock = new RealtimeClock();
            //_recorder = new HEVCRecorder(_videoWidth, _videoHeight, _frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            _recorder = new HEVCRecorder(_videoWidth, _videoHeight, _frameRate, sampleRate, channelCount);
            if (_watermarkEnabled)
            {
                _watermarkInput = new WatermarkTextureInput(_recorder);
                _watermarkInput.watermark = _watermarkImage;
                if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono)
                {
                    _watermarkInput.rect = new(Mathf.RoundToInt(_videoWidth * WatermarkPortraitPosXRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkPortraitPosYRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkPortraitHeightRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkPortraitHeightRatio));
                }
                else
                {
                    _watermarkInput.rect = new(Mathf.RoundToInt(_videoWidth * WatermarkLandscapePosXRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkLandscapePosYRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkLandscapeHeightRatio),
                                               Mathf.RoundToInt(_videoHeight * WatermarkLandscapeHeightRatio));
                }
                _cameraInput = new(_watermarkInput, clock, _recordCamera);
            }
            else
            {
                _watermarkInput = null;
                _cameraInput = new(_recorder, clock, _recordCamera);
            }
            _cameraInput.HDR = true;
            if (_recordMicrophone)
            {
                _audioInput = new AudioInput(_recorder, clock, _microphoneAudioSource, true);
            }
            else
            {
                _audioInput = new AudioInput(_recorder, clock, _cameraAudioListener);
            }
        }

        public async void StopRecording()
        {
            _watermarkInput?.Dispose();
            _cameraInput?.Dispose();
            _audioInput?.Dispose();
            var path = await _recorder.FinishWriting();
            Debug.Log($"Saved recording to: {path}");

            // Save to Photo Library
            var payload = new NatSuite.Sharing.SavePayload();
            payload.AddMedia(path);
            await payload.Commit();

            // Close the screen AR camera if necessary
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = false;
                _recordCamera.enabled = false;
            }

            if (_recordMicrophone)
            {
                _microphoneAudioSource.Stop();
                Microphone.End(null);
                Destroy(_microphoneAudioSource);
            }

            // Release inputs
            _watermarkInput = null;
            _cameraInput = null;
            _audioInput = null;
        }
    }
}
