using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTab_Display : HoloKitAppUIRealitySettingTab
    {
        public override string TabName => "Display";

        [SerializeField] private TMP_Text _occlusionStatus;

        private void Start()
        {
            
        }

        public void OnOcclusionToggleValueChanged(bool toggled)
        {
            _occlusionStatus.text = toggled ? "On" : "Off";
            HoloKitAppUIEventManager.OnHumanOcclusionToggled?.Invoke(toggled);
        }
    }
}
