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
            RealityManager.OnRealityManagerSpawned += OnLocalClientConnected;
        }

        private void OnDestroy()
        {
            RealityManager.OnRealityManagerSpawned -= OnLocalClientConnected;
        }

        private void OnLocalClientConnected()
        {
            HoloKitAppUIPanelManager.Instance.PushUIPanel("MonoAR_ScanQRCode");
        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
            HoloKitAppUIEventManager.OnExit?.Invoke();
        }
    }
}
