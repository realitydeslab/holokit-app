using System.Collections;
using UnityEngine;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppRecorder : MonoBehaviour
    {
        [Header(@"Recording")]
        [HideInInspector] public int VideoWidth = 1170;
        [HideInInspector] public int VideoHeight = 2532;
        [HideInInspector] public Camera RecordCamera;
        public bool RecordMicrophone;

        private MP4Recorder _recorder;
        private CameraInput _cameraInput;
        private AudioInput _audioInput;
        private AudioSource _microphoneSource;

        private IEnumerator Start()
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

        private void OnDestroy()
        {
            // Stop microphone
            _microphoneSource.Stop();
            Microphone.End(null);
        }

        private void PrepareRecorderParams()
        {
            VideoWidth = Screen.width;
            VideoHeight = Screen.height;

            RecordCamera = HoloKitCamera.Instance.GetComponent<Camera>();
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                RecordCamera.enabled = true;
            }
        }

        public void StartRecording()
        {
            PrepareRecorderParams();

            // Start recording
            var frameRate = 30;
            var sampleRate = RecordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = RecordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var clock = new RealtimeClock();
            _recorder = new MP4Recorder(VideoWidth, VideoHeight, frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            // Create recording inputs
            _cameraInput = new CameraInput(_recorder, clock, RecordCamera);
            _cameraInput.HDR = true;
            _audioInput = RecordMicrophone ? new AudioInput(_recorder, clock, _microphoneSource, true) : null;
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

            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                RecordCamera.enabled = false;
            }

            // Save to Photo Library
            var payload = new NatSuite.Sharing.SavePayload();
            payload.AddMedia(path);
            await payload.Commit();
        }
    }
}
