namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_LandingPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "LandingPage";

        public override bool OverlayPreviousPanel => true;

        private const float Duration = 3.6f;

        private void Start()
        {
            StartCoroutine(HoloKitAppUtils.WaitAndDo(Duration, () =>
            {
                LoadPermissionPage();
            }));
        }

        private void LoadPermissionPage()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("PermissionsPage");
        }
    }
}
