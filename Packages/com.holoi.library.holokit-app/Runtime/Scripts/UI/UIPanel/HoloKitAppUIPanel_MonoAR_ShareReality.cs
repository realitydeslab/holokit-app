namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ShareReality : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ShareReality";

        public override bool OverlayPreviousPanel => true;

        public void OnShareRealityButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
            HoloKitAppUIEventManager.OnStartedAdvertising?.Invoke();
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
