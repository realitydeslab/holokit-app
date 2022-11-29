using UnityEngine;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject Scores;

        [SerializeField] protected GameObject Time;

        [SerializeField] protected GameObject Reticle;

        [SerializeField] protected GameObject Status;

        [SerializeField] protected GameObject RedScreen;

        [Header("Settings"), Tooltip("Will this panel rotate with the device orientation?")]
        [SerializeField] private bool _autoRotate = true;

        private Canvas _canvas;

        private DeviceOrientation _deviceOrientation = DeviceOrientation.Portrait;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();

            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            HoloKitAppRecorder.OnStartedRecording += OnStartedRecording;
            HoloKitAppRecorder.OnStoppedRecording += OnStoppedRecording;
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;

            Scores.SetActive(false);
            Time.SetActive(false);
            Reticle.SetActive(false);
            Status.SetActive(false);
            RedScreen.SetActive(false);
        }

        private void Update()
        {
            if (!_autoRotate || Screen.orientation == ScreenOrientation.LandscapeLeft) return;

            if (Input.deviceOrientation != _deviceOrientation)
            {
                _deviceOrientation = Input.deviceOrientation;
                OnDeviceOrientationChanged();
            }
        }

        private void OnDeviceOrientationChanged()
        {
            if (_deviceOrientation == DeviceOrientation.Portrait)
            {
                GetComponent<RectTransform>().localRotation = Quaternion.identity;
            }
            else if (_deviceOrientation == DeviceOrientation.LandscapeLeft)
            {
                GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, -90f);
            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                GetComponent<RectTransform>().localRotation = Quaternion.identity;
                _canvas.renderMode = RenderMode.WorldSpace;
            }
            else if (renderMode == HoloKitRenderMode.Mono)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            HoloKitAppRecorder.OnStartedRecording -= OnStartedRecording;
            HoloKitAppRecorder.OnStoppedRecording -= OnStoppedRecording;
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    OnCountdown();
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    break;
                case MofaPhase.RoundResult:
                    break;
                case MofaPhase.RoundData:
                    OnRoundData();
                    break;
            }
        }

        protected virtual void OnCountdown()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator) // Spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
            }
            else // Not spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
                Reticle.SetActive(true);
                Status.SetActive(true);
                RedScreen.SetActive(true);
            }
        }

        protected virtual void OnRoundData()
        {
            Scores.SetActive(false);
            Time.SetActive(false);
            Reticle.SetActive(false);
            Status.SetActive(false);
        }

        private void OnStartedRecording()
        {
            GetComponent<RectTransform>().localPosition = new Vector3(0f, 99f, 1f);
        }

        private void OnStoppedRecording()
        {
            GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
        }
    }
}