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
            // TODO: Go to holokit.io Safari
        }

        public void OnBuyHoloKitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("BuyHoloKitPage");
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

        }

        public void OnDiscordButtonPressed()
        {

        }

        public void OnInstagramButtonPressed()
        {

        }

        public void OnTiktokButtonPressed()
        {

        }

        public void OnYoutubeButtonPressed()
        {

        }

        public void OnMirrorButtonPressed()
        {

        }
    }
}
