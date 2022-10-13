using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppRecorder : MonoBehaviour
    {
        [Header(@"Recording")]
        [SerializeField] private bool _recordMicrophone;

        [SerializeField] private Camera _watermarkCamera;

        [SerializeField] private GameObject _watermark;

        public bool IsRecording => _cameraInput != null;

        private int _videoWidth = 1170;

        private int _videoHeight = 2532;

        private readonly Vector3 _watermarkPortraitLocalPosition = new(-0.15f, -0.45f, 1f);

        private readonly Vector3 _watermarkLandscapeLocalPosition = new(-1f, -0.45f, 1f);

        private const float WatermarkPortraitLocalScale = 0.08f;

        private const float WatermarkLandscapeLocalScale = 0.1f;

        private Camera _recordCamera;

        private MP4Recorder _recorder;

        private CameraInput _cameraInput;

        private AudioInput _audioInput;

        private AudioSource _microphoneSource;

        private IEnumerator Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                // Start microphone
                _microphoneSource = gameObject.AddComponent<AudioSource>();
                _microphoneSource.mute =
                _microphoneSource.loop = true;
                _microphoneSource.bypassEffects =
                _microphoneSource.bypassListenerEffects = false;
                _microphoneSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
                yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
                _microphoneSource.Play();
            }
        }

        private void OnDestroy()
        {
            // Stop microphone
            _microphoneSource.Stop();
            Microphone.End(null);
        }

        private void PrepareRecorderParams()
        {
            _videoWidth = Screen.width;
            _videoHeight = Screen.height;

            _recordCamera = HoloKitCamera.Instance.GetComponent<Camera>();
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = true;
                _recordCamera.enabled = true;
                _watermark.transform.localPosition = _watermarkLandscapeLocalPosition;
                _watermark.transform.localScale = new Vector3(WatermarkLandscapeLocalScale,
                                                              WatermarkLandscapeLocalScale,
                                                              WatermarkLandscapeLocalScale);
            }
            else
            {
                _watermark.transform.localPosition = _watermarkPortraitLocalPosition;
                _watermark.transform.localScale = new Vector3(WatermarkPortraitLocalScale,
                                                              WatermarkPortraitLocalScale,
                                                              WatermarkPortraitLocalScale);
            }
            _watermarkCamera.gameObject.SetActive(true);
            var cameraData = _recordCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Clear();
            cameraData.cameraStack.Add(_watermarkCamera);
        }

        private void ReleaseRecorderParams()
        {
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = false;
                _recordCamera.enabled = false;
            }
            _watermarkCamera.gameObject.SetActive(false);
        }

        public void StartRecording()
        {
            PrepareRecorderParams();

            // Start recording
            var frameRate = 30;
            var sampleRate = _recordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = _recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var clock = new RealtimeClock();
            _recorder = new MP4Recorder(_videoWidth, _videoHeight, frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            // Create recording inputs
            _cameraInput = new(_recorder, clock, new Camera[] { _recordCamera, _watermarkCamera });
            _cameraInput.HDR = true;
            _audioInput = _recordMicrophone ? new AudioInput(_recorder, clock, _microphoneSource, true) : null;
            // Unmute microphone
            _microphoneSource.mute = _audioInput == null;
        }

        public async void StopRecording()
        {
            // Mute microphone
            _microphoneSource.mute = true;
            // Stop recording
            _audioInput?.Dispose();
            _cameraInput.Dispose();
            var path = await _recorder.FinishWriting();
            // Playback recording
            Debug.Log($"Saved recording to: {path}");

            // Save to Photo Library
            var payload = new NatSuite.Sharing.SavePayload();
            payload.AddMedia(path);
            await payload.Commit();

            ReleaseRecorderParams();
        }
    }
}
