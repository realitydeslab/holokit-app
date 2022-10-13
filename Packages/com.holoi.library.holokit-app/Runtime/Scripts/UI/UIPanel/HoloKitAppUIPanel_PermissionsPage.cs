namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_PermissionsPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "PermissionsPage";

        public override bool OverlayPreviousPanel => true;

        private void Start()
        {
            if (HoloKitAppPermissionsManager.MandatoryPermissionsGranted())
            {
                HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("SignInPage");
            }
        }
    }
}
