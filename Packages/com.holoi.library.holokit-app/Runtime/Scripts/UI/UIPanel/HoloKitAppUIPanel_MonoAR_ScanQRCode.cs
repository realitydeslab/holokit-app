using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ScanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ScanQRCode";

        public override bool OverlayPreviousPanel => true;

        private void Start()
        {
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated += OnConnectedPlayerListUpdated;
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated -= OnConnectedPlayerListUpdated;
        }

        private void OnConnectedPlayerListUpdated()
        {
            var localPlayer = HoloKitApp.Instance.MultiplayerManager.LocalPlayer;
            if (localPlayer.SyncStatus == HoloKitAppPlayerSyncStatus.Synced)
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
