namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_RescanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_RescanQRCode";

        public override bool OverlayPreviousPanel => true;

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnRescanButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
            HoloKitAppUIEventManager.OnRescanQRCode?.Invoke();
        }
    }
}
