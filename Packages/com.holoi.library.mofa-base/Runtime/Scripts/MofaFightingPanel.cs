using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Library.MOFABase
{
    public struct MofaFightingPanelParams
    {
        public float RotatorScale;
        public float HeaderPosY;
        public float HeaderScale;
        public float StatusPosX;
        public float StatusPosY;
        public float StatusScale;
    }

    public class MofaFightingPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected RectTransform Rotator;

        [SerializeField] protected RectTransform Scores;

        [SerializeField] protected RectTransform Time;

        [SerializeField] protected RectTransform Reticle;

        [SerializeField] protected RectTransform Status;

        [SerializeField] protected RectTransform RedScreen;

        [Header("Settings"), Tooltip("Will this panel rotate with the device orientation?")]
        [SerializeField] private bool _autoRotate = true;

        private Canvas _canvas;

        private DeviceOrientation _deviceOrientation = DeviceOrientation.Portrait;

        private readonly MofaFightingPanelParams _monoPortraitParams = new()
        {
            RotatorScale = 1f,
            HeaderPosY = 800f,
            HeaderScale = 1f,
            StatusPosX = 240f,
            StatusPosY = -720f,
            StatusScale = 3000f
        };

        private readonly MofaFightingPanelParams _monoLandscapeParams = new()
        {
            RotatorScale = 1f,
            HeaderPosY = 460f,
            HeaderScale = 1f,
            StatusPosX = 240f,
            StatusPosY = -460f,
            StatusScale = 3000f
        };

        private readonly MofaFightingPanelParams _starParams = new()
        {
            RotatorScale = 1.2f,
            HeaderPosY = 360f,
            HeaderScale = 1f,
            StatusPosX = 240f,
            StatusPosY = -460f,
            StatusScale = 3000f
        };

        private void Start()
        {
            _canvas = GetComponent<Canvas>();

            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            HoloKitAppRecorder.OnRecordingStarted += OnRecordingStarted;
            HoloKitAppRecorder.OnRecordingStopped += OnRecordingStopped;
            HoloKitAppUIPanel_MonoAR_ShareQRCode.OnStartedSharingQRCode += OnRecordingStarted;
            HoloKitAppUIPanel_MonoAR_ShareQRCode.OnStoppedSharingQRCode += OnRecordingStopped;
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
            LifeShield.OnSpawned += OnLifeShieldSpawned;

            Scores.gameObject.SetActive(false);
            Time.gameObject.SetActive(false);
            Reticle.gameObject.SetActive(false);
            Status.gameObject.SetActive(false);
            RedScreen.gameObject.SetActive(false);
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

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            HoloKitAppRecorder.OnRecordingStarted -= OnRecordingStarted;
            HoloKitAppRecorder.OnRecordingStopped -= OnRecordingStopped;
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
            LifeShield.OnSpawned -= OnLifeShieldSpawned;
        }

        private void OnDeviceOrientationChanged()
        {
            if (_deviceOrientation == DeviceOrientation.Portrait)
            {
                Rotator.localRotation = Quaternion.identity;
                UpdateMofaFightingPanelParams(_monoPortraitParams);
            }
            else if (_deviceOrientation == DeviceOrientation.LandscapeLeft)
            {
                Rotator.localRotation = Quaternion.Euler(0f, 0f, -90f);
                UpdateMofaFightingPanelParams(_monoLandscapeParams);
            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
                Rotator.localRotation = Quaternion.identity;
                UpdateMofaFightingPanelParams(_starParams);
            }
            else if (renderMode == HoloKitRenderMode.Mono)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = HoloKitCamera.Instance.GetComponent<Camera>();
                _deviceOrientation = Input.deviceOrientation;
                OnDeviceOrientationChanged();
            }
        }

        private void UpdateMofaFightingPanelParams(MofaFightingPanelParams fightingPanelParams)
        {
            Rotator.localScale = new(fightingPanelParams.RotatorScale, fightingPanelParams.RotatorScale, fightingPanelParams.RotatorScale);
            Scores.anchoredPosition = new(0f, fightingPanelParams.HeaderPosY);
            Scores.localScale = new(fightingPanelParams.HeaderScale, fightingPanelParams.HeaderScale, fightingPanelParams.HeaderScale);
            Time.anchoredPosition = new(0f, fightingPanelParams.HeaderPosY);
            Time.localScale = new(fightingPanelParams.HeaderScale, fightingPanelParams.HeaderScale, fightingPanelParams.HeaderScale);
            Status.anchoredPosition = new(fightingPanelParams.StatusPosX, fightingPanelParams.StatusPosY);
            Status.localScale = new(fightingPanelParams.StatusScale, fightingPanelParams.StatusScale, fightingPanelParams.StatusScale);
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
                Scores.gameObject.SetActive(true);
                Time.gameObject.SetActive(true);
            }
            else // Not spectator
            {
                Scores.gameObject.SetActive(true);
                Time.gameObject.SetActive(true);
                Reticle.gameObject.SetActive(true);
                Status.gameObject.SetActive(true);
                RedScreen.gameObject.SetActive(true);
            }
        }

        protected virtual void OnRoundData()
        {
            Scores.gameObject.SetActive(false);
            Time.gameObject.SetActive(false);
            Reticle.gameObject.SetActive(false);
            Status.gameObject.SetActive(false);
        }

        private void OnRecordingStarted()
        {
            Rotator.anchoredPosition = new Vector2(0f, 3000f);
        }

        private void OnRecordingStopped()
        {
            Rotator.anchoredPosition = Vector2.zero;
        }

        private void OnLifeShieldSpawned(LifeShield lifeShield)
        {
            if (lifeShield.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                Status.GetComponent<MofaFightingPanelStatus>().SetLifeShield(lifeShield);
            }
        }
    }
}