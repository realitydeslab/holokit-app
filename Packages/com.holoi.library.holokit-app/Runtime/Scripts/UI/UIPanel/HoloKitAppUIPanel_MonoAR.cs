using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private HoloKitAppUIComponent_MonoAR_RecordButton _recordButton;

        private void Start()
        {
            // If local device is spectator
            if (!HoloKitApp.Instance.IsHost)
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_WaitingForConnection");
            }
        }

        public void OnSpectatorButtonPressed()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ShareReality");
            }
            else
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_RescanQRCode");
            }
        }

        public void OnStarButtonPressed()
        {
            // Enter Star Mode
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("StarAR");
            HoloKitAppUIEventManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Stereo);
        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIEventManager.OnExit?.Invoke();
        }

        public void OnRecordButtonPressed()
        {
            _recordButton.ToggleRecording();
        }
    }
}
