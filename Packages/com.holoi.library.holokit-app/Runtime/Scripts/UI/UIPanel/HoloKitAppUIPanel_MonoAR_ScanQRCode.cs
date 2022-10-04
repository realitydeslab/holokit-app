using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ScanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ScanQRCode";

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
            HoloKitAppUIPanelManager.Instance.PushUIPanel("MonoAR_CheckAlignmentMark");
        }
    }
}
