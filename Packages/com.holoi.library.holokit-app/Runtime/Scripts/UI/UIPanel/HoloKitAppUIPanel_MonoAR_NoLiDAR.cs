namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_NoLiDAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_NoLiDAR";

        public override bool OverlayPreviousPanel => true;

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.ExitNoLiDARScene();
        }
    }
}
