namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_CheckAlignmentMark : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_CheckAlignmentMark";

        public override bool OverlayPreviousPanel => true;

        public void OnCheckedButtonPressed()
        {
            while (!HoloKitApp.Instance.UIPanelManager.PeekUIPanel().Equals("MonoAR"))
            {
                HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            }
            HoloKitAppUIEventManager.OnAlignmentMarkChecked?.Invoke();
        }

        public void OnRescanButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnRescanQRCode?.Invoke();
        }
    }
}
