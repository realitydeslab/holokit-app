using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_CheckAlignmentMark : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_CheckAlignmentMark";

        public override bool OverlayPreviousPanel => true;

        public void OnCheckedButtonPressed()
        {
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
            HoloKitAppUIEventManager.OnAlignmentMarkChecked?.Invoke();
        }

        public void OnRescanButtonPressed()
        {
            HoloKitAppUIPanelManager.Instance.PopUIPanel();
            HoloKitAppUIEventManager.OnRescanQRCode?.Invoke();
        }
    }
}
