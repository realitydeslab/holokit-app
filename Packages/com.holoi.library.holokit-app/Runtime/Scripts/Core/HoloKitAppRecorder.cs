using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using NatML.VideoKit;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public class HoloKitAppRecorder : MonoBehaviour
    {
        [SerializeField] private Texture _watermarkImage;

        public bool IsRecording => _isRecording;

        private VideoKitRecorder _videokitRecorder;

        private Camera _monoCamera;

        /// <summary>
        /// Whether a recording is currently in progress.
        /// </summary>
        private bool _isRecording;

        private const float WatermarkPortraitPosXRatio = 0.0615f;

        private const float WatermarkPortraitPosYRatio = 0.0284f;

        private const float WatermarkPortraitWidthRatio = 0.2949f;

        private const float WatermarkPortraitHeightRatio = 0.0284f;

        private const float WatermarkLandscapePosXRatio = 0.0284f;

        private const float WatermarkLandscapePosYRatio = 0.0615f;

        private const float WatermarkLandscapeWidthRatio = 0.1363f;

        private const float WatermarkLandscapeHeightRatio = 0.0615f;

        public static event Action OnRecordingStarted;

        public static event Action OnRecordingCompleted;

        public static event Action OnRecordingFailed;

        private void Start()
        {
            _videokitRecorder = GetComponent<VideoKitRecorder>();

            // Setup the recording camera. We will only use the mono camera for recording.
            Camera monoCamera = HoloKitCamera.Instance.GetComponent<Camera>();
            _videokitRecorder.cameras = new Camera[] { HoloKitCamera.Instance.GetComponent<Camera>() };

            // Microphone settings
            if (HoloKitApp.Instance.GlobalSettings.RecordMicrophone)
                _videokitRecorder.audioMode = VideoKitRecorder.AudioMode.AudioDevice;
            else
                _videokitRecorder.audioMode = VideoKitRecorder.AudioMode.AudioListener;

            // Watermark settings


            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void OnDestroy()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
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

        private void StartRecordingInternal()
        {
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _monoCamera.GetComponent<ARCameraBackground>().enabled = true;
                _monoCamera.enabled = true;
            }

            _videokitRecorder.StartRecording();
            _isRecording = true;

            OnRecordingStarted?.Invoke();
        }

        public void StopRecording()
        {
            if (HoloKitUtils.IsEditor)
            {
                Debug.Log("[HoloKitAppRecorder] Cannot record in editor mode");
                return;
            }

            _videokitRecorder.StopRecording();
        }

        public void OnRecordingCompletedInternal(string dest)
        {
            OnRecordingStoppedInternal();
            OnRecordingCompleted?.Invoke();
        }

        public void OnRecordingFailedInternal(Exception e)
        {
            OnRecordingStoppedInternal();
            OnRecordingFailed?.Invoke();
        }

        private void OnRecordingStoppedInternal()
        {
            _isRecording = false;
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                _monoCamera.GetComponent<ARCameraBackground>().enabled = false;
                _monoCamera.enabled = false;
            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (_isRecording)
            {
                StopRecording();
                Debug.Log("[Recorder] Recording interrupted due to a render mode change event");
            }
        }
    }
}
