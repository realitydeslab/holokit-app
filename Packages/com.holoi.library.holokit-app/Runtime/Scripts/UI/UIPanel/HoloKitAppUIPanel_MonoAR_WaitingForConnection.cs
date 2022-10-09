using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_WaitingForConnection : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_WaitingForConnection";

        public override bool OverlayPreviousPanel => true;

        private void Awake()
        {
            HoloKitAppMultiplayerManager.OnLocalClientConnected += OnLocalClientConnected;
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnLocalClientConnected -= OnLocalClientConnected;
        }

        private void OnLocalClientConnected()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnExit?.Invoke();
        }
    }
}
