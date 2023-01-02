using System;
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

    public enum MofaFightingPanelMode
    {
        MonoPortrait = 0,
        MonoLandscape = 1,
        Star = 2
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

        protected virtual void Start()
        {
            _canvas = GetComponent<Canvas>();

            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;
            HoloKitCamera.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;

            Scores.gameObject.SetActive(false);
            Time.gameObject.SetActive(false);
            Reticle.gameObject.SetActive(false);
            Status.gameObject.SetActive(false);
            RedScreen.gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
            HoloKitCamera.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void Update()
        {
            // Auto screen orientation check
            if (_autoRotate && Screen.orientation == ScreenOrientation.Portrait)
            {
                if (Input.deviceOrientation != _deviceOrientation)
                {
                    _deviceOrientation = Input.deviceOrientation;
                    OnDeviceOrientationChanged();
                }
            }

            CheckAppearance();
        }

        private void OnDeviceOrientationChanged()
        {
            if (_deviceOrientation == DeviceOrientation.Portrait)
            {
                Rotator.localRotation = Quaternion.identity;
                UpdateMofaFightingPanelParams(_monoPortraitParams);
                OnMofaFightingPanelModeChanged(MofaFightingPanelMode.MonoPortrait);
            }
            else if (_deviceOrientation == DeviceOrientation.LandscapeLeft)
            {
                Rotator.localRotation = Quaternion.Euler(0f, 0f, -90f);
                UpdateMofaFightingPanelParams(_monoLandscapeParams);
                OnMofaFightingPanelModeChanged(MofaFightingPanelMode.MonoLandscape);
            }
        }

        private void CheckAppearance()
        {
            var holokitApp = HoloKitApp.HoloKitApp.Instance;
            if (holokitApp.Recorder.IsRecording)
            {
                OnDisappear();
                return;
            }

            if (holokitApp.IsHost)
            {
                if (holokitApp.MultiplayerManager.IsAdvertising)
                {
                    OnDisappear();
                    return;
                }
            }
            else
            {
                var localPlayer = holokitApp.MultiplayerManager.LocalPlayer;
                if (localPlayer != null && localPlayer.PlayerStatus.Value != HoloKitAppPlayerStatus.Checked)
                {
                    OnDisappear();
                    return;
                }
            }

            OnAppear();
        }

        private void OnDisappear()
        {
            Rotator.anchoredPosition = new Vector2(0f, 3000f);
        }

        private void OnAppear()
        {
            Rotator.anchoredPosition = Vector2.zero;
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
                Rotator.localRotation = Quaternion.identity;
                UpdateMofaFightingPanelParams(_starParams);
                OnMofaFightingPanelModeChanged(MofaFightingPanelMode.Star);
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

        private void OnMofaPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    OnRoundData();
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

        protected virtual void OnMofaFightingPanelModeChanged(MofaFightingPanelMode mode)
        {

        }
    }
}