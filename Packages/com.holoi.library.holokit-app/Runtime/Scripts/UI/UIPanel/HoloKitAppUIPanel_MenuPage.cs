using UnityEngine;
using Holoi.Library.HoloKitApp.IOSNative;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MenuPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MenuPage";

        public override bool OverlayPreviousPanel => true;

        private void OnEnable()
        {
            Camera.main.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }

        private void OnDisable()
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = new Color(1f, 1f, 1f, 0f);
            }
        }

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
            //Application.OpenURL("https://holokit.io");
            HoloKitAppIOSNativeAPI.OpenURL("https://holokit.io");
        }

        public void OnBuyHoloKitButtonPressed()
        {
            //HoloKitApp.Instance.UIPanelManager.PushUIPanel("BuyHoloKitPage");
            //Application.OpenURL("https://holokit.io/x/get-it-now/");
            HoloKitAppIOSNativeAPI.OpenURL("https://holokit.io/x/get-it-now/");
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

        public void OnFeedbackButtonPressed()
        {
            HoloKitAppIOSNativeAPI.ShowFeedbackAlert();
        }

        public void OnTwitterButtonPressed()
        {
            //Application.OpenURL("https://twitter.com/holokit_io");
            HoloKitAppIOSNativeAPI.OpenURL("https://twitter.com/holokit_io");
        }

        public void OnDiscordButtonPressed()
        {
            //Application.OpenURL("https://discord.gg/nsPPBfAJ2f");
            HoloKitAppIOSNativeAPI.OpenURL("https://discord.gg/nsPPBfAJ2f");
        }

        public void OnInstagramButtonPressed()
        {
            //Application.OpenURL("https://www.instagram.com/holokit.io/");
            HoloKitAppIOSNativeAPI.OpenURL("https://www.instagram.com/holokit.io/");
        }

        public void OnTiktokButtonPressed()
        {
            //Application.OpenURL("https://www.tiktok.com/@holokit.io");
            HoloKitAppIOSNativeAPI.OpenURL("https://www.tiktok.com/@holokit.io");
        }

        public void OnYoutubeButtonPressed()
        {
            //Application.OpenURL("https://www.youtube.com/channel/UCNLdtATBRfPlKbdsTb1Y-5Q");
            HoloKitAppIOSNativeAPI.OpenURL("https://www.youtube.com/channel/UCNLdtATBRfPlKbdsTb1Y-5Q");
        }

        public void OnMirrorButtonPressed()
        {
            //Application.OpenURL("https://holokit.io/x/get_it_now/");
        }
    }
}
