using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_StarAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "StarAR";

        public override bool OverlayPreviousPanel => true;

        private void Awake()
        {
            int a = Screen.width;
            int b = Screen.height;
            GetComponent<RectTransform>().sizeDelta = new Vector3(a > b ? a : b, a < b ? a : b);
        }
    }
}
