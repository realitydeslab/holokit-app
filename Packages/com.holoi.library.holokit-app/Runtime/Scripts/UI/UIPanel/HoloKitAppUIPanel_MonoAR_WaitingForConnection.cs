using UnityEngine;
using Netcode.Transports.MultipeerConnectivity;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_WaitingForConnection : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_WaitingForConnection";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _text;

        private string _peerName;

        private void Start()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer += OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer += OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer += OnConnectingWithPeer;
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated += OnConnectedPlayerListUpdated;
        }

        private void OnDestroy()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
            HoloKitAppMultiplayerManager.OnConnectedPlayerListUpdated -= OnConnectedPlayerListUpdated;
        }

        private void OnBrowserFoundPeer(string peerName)
        {
            _peerName = peerName;
            _text.text = $"Found nearby host\n{peerName}...";
        }

        private void OnBrowserLostPeer(string peerName)
        {
            _peerName = peerName;
            _text.text = $"Lost nearby host\n{peerName}";
        }

        private void OnConnectingWithPeer(string peerName)
        {
            _peerName = peerName;
            _text.text = $"Connecting to nearby\nhost {peerName}...";
        }

        private void OnConnectedPlayerListUpdated()
        {
            var localPlayer = HoloKitApp.Instance.MultiplayerManager.LocalPlayer;
            switch (localPlayer.SyncStatus)
            {
                case HoloKitAppPlayerSyncStatus.None:
                    _text.text = $"Connected to nearby\nhost {_peerName}";
                    break;
                case HoloKitAppPlayerSyncStatus.SyncingTimestamp:
                    _text.text = $"Syncing timestamp with\nnearby host {_peerName}...";
                    break;
                case HoloKitAppPlayerSyncStatus.SyncingPose:
                    HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                    HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
                    break;
                case HoloKitAppPlayerSyncStatus.Synced:
                    break;
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnExitReality?.Invoke();
        }
    }
}
