using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityDetailPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityDetailPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _realityIndexText;

        [SerializeField] private TMP_Text _realityNameText;

        [SerializeField] private TMP_Text _realityAuthorText;

        [SerializeField] private TMP_Text _realityDescriptionText;

        private void Start()
        {
            _realityIndexText.text = "Reality #" +
                HoloKitAppUtils.IntToStringF3(HoloKitApp.Instance.GlobalSettings.GetRealityIndex(HoloKitApp.Instance.CurrentReality) + 1);
            _realityNameText.text = HoloKitApp.Instance.CurrentReality.displayName;
            _realityAuthorText.text = HoloKitApp.Instance.CurrentReality.author;
            _realityDescriptionText.text = HoloKitApp.Instance.CurrentReality.description;
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
