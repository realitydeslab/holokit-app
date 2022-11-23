using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _spectatorButton;

        [SerializeField] private GameObject _starButton;

        [SerializeField] private GameObject _exitButton;

        [SerializeField] private GameObject _starModeHelper;

        [SerializeField] private HoloKitAppUIComponent_MonoAR_RecordButton _recordButton;

        private void Start()
        {
            // If local device is spectator
            if (HoloKitUtils.IsRuntime)
            {
                if (!HoloKitApp.Instance.IsHost)
                {
                    HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_WaitingForConnection");
                }
            }
        }

        public void OnSpectatorButtonPressed()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                //HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ShareReality");
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
            }
            else
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_RescanQRCode");
            }
        }

        public void OnStarButtonPressed()
        {
            // Is the device supported by HoloKitX?
            if (!HoloKitOpticsAPI.IsCurrentDeviceSupportedByHoloKit())
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_IncompatiblePhoneModel");
                return;
            }

            // Enter Star Mode
            if (HoloKitApp.Instance.GlobalSettings.InstructionEnabled && HoloKitUtils.IsRuntime)
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_GettingStarted");
            }
            else
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("StarAR", HoloKitAppUICanvasType.StAR);
                HoloKitAppUIEventManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Stereo);
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIEventManager.OnExitReality?.Invoke();
        }

        public void OnRecordButtonPressed()
        {
            _recordButton.ToggleRecording();
            if (_recordButton.IsRecording)
            {
                _spectatorButton.SetActive(false);
                _starButton.SetActive(false);
                _exitButton.SetActive(false);
                _starModeHelper.SetActive(false);
            }
            else
            {
                _spectatorButton.SetActive(true);
                _starButton.SetActive(true);
                _exitButton.SetActive(true);
            }
        }

        public void OnHamburgerButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_RealitySettings");
        }
    }
}
