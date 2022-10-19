using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ScanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ScanQRCode";

        public override bool OverlayPreviousPanel => true;

        private void Awake()
        {
            HoloKitAppMultiplayerManager.OnFinishedScanningQRCode += OnFinishedScanningQRCode;
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnFinishedScanningQRCode -= OnFinishedScanningQRCode;
        }

        private void Start()
        {
            if (HoloKitUtils.IsEditor)
            {
                OnFinishedScanningQRCode();
            }
        }

        private void OnFinishedScanningQRCode()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_CheckAlignmentMark");
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.Shutdown();
        }
    }
}
