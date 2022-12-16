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

        private string _masterName;

        private void Start()
        {
            // Before the network connection
            MultipeerConnectivityTransport.OnBrowserFoundPeer += OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer += OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer += OnConnectingWithPeer;
            // After the network connection
            HoloKitAppMultiplayerManager.OnLocalPlayerConnected += OnLocalPlayerConnected;
            HoloKitAppMultiplayerManager.OnLocalPlayerDisconnected += OnLocalPlayerDisconnected;
            HoloKitAppMultiplayerManager.OnLocalPlayerSyncingTimestamp += OnLocalPlayerSyncingTimestamp;
            HoloKitAppMultiplayerManager.OnLocalPlayerSyncingPose += OnLocalPlayerSyncingPose;
            HoloKitAppMultiplayerManager.OnLocalPlayerChecked += OnLocalPlayerChecked;

            // Start browsing manually
            StartCoroutine(HoloKitAppUtils.WaitAndDo(1f, () =>
            {
                MultipeerConnectivityTransport.StartBrowsing();
            }));
        }

        private void OnDestroy()
        {
            // Before the network connection
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
            // After the network connection
            HoloKitAppMultiplayerManager.OnLocalPlayerConnected -= OnLocalPlayerConnected;
            HoloKitAppMultiplayerManager.OnLocalPlayerDisconnected -= OnLocalPlayerDisconnected;
            HoloKitAppMultiplayerManager.OnLocalPlayerSyncingTimestamp -= OnLocalPlayerSyncingTimestamp;
            HoloKitAppMultiplayerManager.OnLocalPlayerSyncingPose -= OnLocalPlayerSyncingPose;
            HoloKitAppMultiplayerManager.OnLocalPlayerChecked -= OnLocalPlayerChecked;
        }

        private void OnBrowserFoundPeer(string peerName)
        {
            _masterName = peerName;
            _text.text = $"Found nearby host\n{peerName}...";
        }

        private void OnBrowserLostPeer(string peerName)
        {
            _text.text = $"Lost nearby host\n{peerName}";
        }

        private void OnConnectingWithPeer(string peerName)
        {
            _masterName = peerName;
            _text.text = $"Connecting to nearby\nhost {peerName}...";
        }

        private void OnLocalPlayerConnected()
        {
            _text.text = $"Connected to nearby\nhost {_masterName}";
        }

        private void OnLocalPlayerDisconnected()
        {
            _text.text = $"Disconnected from nearby\nhost {_masterName}";
        }

        private void OnLocalPlayerSyncingTimestamp()
        {
            _text.text = $"Syncing timestamp with\nnearby host {_masterName}...";
        }

        private void OnLocalPlayerSyncingPose()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
        }

        private void OnLocalPlayerChecked()
        {
            if (HoloKit.HoloKitUtils.IsEditor)
                HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnExitReality?.Invoke();
        }
    }
}
