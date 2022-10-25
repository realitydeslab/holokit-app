using System.Collections;
using UnityEngine;
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

        [SerializeField] private bool _watermarkEnabled;

        public bool IsRecording => _cameraInput != null;

        private int _videoWidth = 1170;

        private int _videoHeight = 2532;

        private Camera _recordCamera;

        private MP4Recorder _recorder;

        private CameraInput _cameraInput;

        private AudioInput _audioInput;

        private AudioSource _microphoneSource;

        private IEnumerator Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                if (_watermarkEnabled)
                {
                    
                }
                else
                {
                    
                }

                if (_recordMicrophone)
                {
                    // Start microphone
                    _microphoneSource = gameObject.AddComponent<AudioSource>();
                    _microphoneSource.mute = false;
                    _microphoneSource.loop = true;
                    _microphoneSource.bypassEffects = false;
                    _microphoneSource.bypassListenerEffects = false;
                    _microphoneSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
                    yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
                    _microphoneSource.Play();
                }
            }
        }

        private void OnDestroy()
        {
            // Stop microphone
            if (_recordMicrophone)
            {

                _microphoneSource.Stop();
                Microphone.End(null);
            }
        }

        private void PrepareRecorderParams()
        {
            _videoWidth = Screen.width;
            _videoHeight = Screen.height;
            if (_videoWidth % 2 != 0)
            {
                _videoWidth--;
            }
            if (_videoHeight %2 != 0)
            {
                _videoHeight--;
            }

            _recordCamera = HoloKitCamera.Instance.GetComponent<Camera>();
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = true;
                _recordCamera.enabled = true;
            }
        }

        private void ReleaseRecorderParams()
        {
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _recordCamera.GetComponent<ARCameraBackground>().enabled = false;
                _recordCamera.enabled = false;
            }
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
            _cameraInput = new(_recorder, clock, new Camera[] { _recordCamera });
            _cameraInput.HDR = true;
            _audioInput = _recordMicrophone ? new AudioInput(_recorder, clock, _microphoneSource, true) : null;
            // Unmute microphone
            if (_recordMicrophone)
            {
                _microphoneSource.mute = _audioInput == null;
            }
        }

        public async void StopRecording()
        {
            // Mute microphone
            if (_recordMicrophone)
            {
                _microphoneSource.mute = true;
            }
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
