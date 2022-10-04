using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareReality : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareReality";

        public override bool OverlayPreviousPanel => true;

        public void OnShareRealityButtonPressed()
        {
            HoloKitAppUIPanelManager.Instance.PushUIPanel("MonoAR_ShareQRCode");
            HoloKitAppUIEventManager.OnStartedAdvertising?.Invoke();
        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
        }
    }
}
