namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_ScanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_ScanQRCode";

        public override bool OverlayPreviousPanel => true;

        private void Awake()
        {
            HoloKitAppMultiplayerManager.OnLocalPlayerSynced += OnLocalPlayerSynced;
        }

        private void OnDestroy()
        {
            HoloKitAppMultiplayerManager.OnLocalPlayerSynced -= OnLocalPlayerSynced;
        }

        private void OnLocalPlayerSynced()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_CheckAlignmentMark");
        }

        //private void Update()
        //{
        //    var localPlayer = HoloKitApp.Instance.MultiplayerManager.LocalPlayer;
        //    if (localPlayer != null)
        //    {
        //        if (localPlayer.PlayerStatus.Value == HoloKitAppPlayerStatus.Synced)
        //        {
        //            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        //            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_CheckAlignmentMark");
        //        }
        //    }
        //}

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.Shutdown();
        }
    }
}
