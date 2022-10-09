using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareQRCode";

        public override bool OverlayPreviousPanel => true;

        public void OnExitButtonPressed()
        {
            HoloKitAppUIEventManager.OnStoppedAdvertising?.Invoke();
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
