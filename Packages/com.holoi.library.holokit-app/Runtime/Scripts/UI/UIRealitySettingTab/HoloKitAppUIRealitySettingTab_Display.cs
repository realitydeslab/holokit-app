using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTab_Display : HoloKitAppUIRealitySettingTab
    {
        public override string TabName => "Display";

        [SerializeField] private TMP_Text _occlusionStatus;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _occlusionToggle;

        private AROcclusionManager _arOcclusionManager;

        private void Start()
        {
            _arOcclusionManager = HoloKitCamera.Instance.GetComponent<AROcclusionManager>();
            if (_arOcclusionManager != null
                && _arOcclusionManager.requestedHumanDepthMode != UnityEngine.XR.ARSubsystems.HumanSegmentationDepthMode.Disabled
                && _arOcclusionManager.requestedHumanStencilMode != UnityEngine.XR.ARSubsystems.HumanSegmentationStencilMode.Disabled
                && _arOcclusionManager.requestedOcclusionPreferenceMode != UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode.PreferHumanOcclusion)
            {
                _occlusionToggle.Toggled = true;
                _occlusionStatus.text = "On";
            }
        }

        public void OnOcclusionToggleValueChanged(bool toggled)
        {
            if (toggled)
            {
                _occlusionStatus.text = "On";
                _arOcclusionManager.requestedHumanDepthMode = UnityEngine.XR.ARSubsystems.HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = UnityEngine.XR.ARSubsystems.HumanSegmentationStencilMode.Fastest;
                _arOcclusionManager.requestedOcclusionPreferenceMode = UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode.PreferHumanOcclusion;
            }
            else
            {
                _occlusionStatus.text = "Off";
                _arOcclusionManager.requestedHumanDepthMode = UnityEngine.XR.ARSubsystems.HumanSegmentationDepthMode.Disabled;
                _arOcclusionManager.requestedHumanStencilMode = UnityEngine.XR.ARSubsystems.HumanSegmentationStencilMode.Disabled;
                _arOcclusionManager.requestedOcclusionPreferenceMode = UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode.NoOcclusion;
            }
        }
    }
}
