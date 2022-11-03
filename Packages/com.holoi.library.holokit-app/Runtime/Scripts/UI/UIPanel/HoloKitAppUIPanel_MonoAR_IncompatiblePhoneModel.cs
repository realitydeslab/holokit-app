namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_IncompatiblePhoneModel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_IncompatiblePhoneModel";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
