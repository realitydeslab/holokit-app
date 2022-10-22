namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityPreferencesPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityPreferencesPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnStartHostButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Host);
        }

        public void OnStartClientButtonPressed()
        {
            HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Spectator);
        }
    }
}
