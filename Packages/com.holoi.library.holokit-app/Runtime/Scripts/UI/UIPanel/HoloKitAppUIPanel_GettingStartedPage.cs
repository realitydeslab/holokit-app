namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_GettingStartedPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "GettingStartedPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnVerifyHoloKitButtonPressed()
        {
            // TODO:
        }

        public void OnOrderHoloKitButtonPressed()
        {
            // TODO:
        }
    }
}
