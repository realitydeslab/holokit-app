using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MenuPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MenuPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnRealitiesButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnIntroButtonPressed()
        {
            Application.OpenURL("https://holokit.io");
        }

        public void OnBuyHoloKitButtonPressed()
        {
            //HoloKitApp.Instance.UIPanelManager.PushUIPanel("BuyHoloKitPage");
            Application.OpenURL("https://holokit.io/x/get_it_now/");
        }

        public void OnGettingStartedButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("GettingStartedPage");
        }

        public void OnMyAccountButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MyAccountPage");
        }

        public void OnSettingsButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("SettingsPage");
        }

        public void OnTwitterButtonPressed()
        {
            Application.OpenURL("https://twitter.com/holokit_io");
        }

        public void OnDiscordButtonPressed()
        {
            //Application.OpenURL("https://holokit.io/x/get_it_now/");
        }

        public void OnInstagramButtonPressed()
        {
            Application.OpenURL("https://www.instagram.com/holokit.io/");
        }

        public void OnTiktokButtonPressed()
        {
            //Application.OpenURL("https://holokit.io/x/get_it_now/");
        }

        public void OnYoutubeButtonPressed()
        {
            Application.OpenURL("https://www.youtube.com/channel/UCNLdtATBRfPlKbdsTb1Y-5Q");
        }

        public void OnMirrorButtonPressed()
        {
            //Application.OpenURL("https://holokit.io/x/get_it_now/");
        }
    }
}
