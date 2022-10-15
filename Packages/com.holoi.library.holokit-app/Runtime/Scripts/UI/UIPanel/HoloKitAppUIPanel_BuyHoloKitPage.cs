namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_BuyHoloKitPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "BuyHoloKitPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnOrderHoloKitButtonPressed()
        {
            // TODO: Jump to Safari
        }
    }
}
