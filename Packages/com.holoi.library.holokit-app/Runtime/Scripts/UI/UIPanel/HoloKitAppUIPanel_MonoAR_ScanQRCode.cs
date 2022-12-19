namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ScanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ScanQRCode";

        public override bool OverlayPreviousPanel => true;

        private void Start()
        {
            HoloKitAppPlayer.OnPlayerStatusChanged += OnPlayerStatusChanged;
        }

        private void OnDestroy()
        {
            HoloKitAppPlayer.OnPlayerStatusChanged -= OnPlayerStatusChanged;
        }

        private void OnPlayerStatusChanged(HoloKitAppPlayer player)
        {
            if (player.IsLocalPlayer && player.Status.Value == HoloKitAppPlayerStatus.Synced)
            {
                HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_CheckAlignmentMark");
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.Shutdown();
        }
    }
}
