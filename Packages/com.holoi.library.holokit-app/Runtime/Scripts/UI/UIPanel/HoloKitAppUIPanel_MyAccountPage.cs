using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MyAccountPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MyAccountPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _appleIdEmailText;

        private const string AppleUserEmailKey = "AppleUserEmail";

        private void Start()
        {
            if (PlayerPrefs.HasKey(AppleUserEmailKey))
            {
                _appleIdEmailText.text = PlayerPrefs.GetString(AppleUserEmailKey);
            }
            else
            {
                _appleIdEmailText.text = "Not found";
            }
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnVerifyHoloKitButtonPressed()
        {
            // TODO:
        }
    }
}
