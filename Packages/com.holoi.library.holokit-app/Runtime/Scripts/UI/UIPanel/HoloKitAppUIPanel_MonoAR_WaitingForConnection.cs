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
            HoloKitAppMultiplayerManager.OnLocalClientConnected += OnLocalClientConnected;
            HoloKitAppMultiplayerManager.OnStartedSyncingTimestamp += OnStartedSyncingTimestamp;
            HoloKitAppMultiplayerManager.OnStartedSyncingPose += OnStartedSyncingPose;
        }

        private void OnDestroy()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
            HoloKitAppMultiplayerManager.OnLocalClientConnected -= OnLocalClientConnected;
            HoloKitAppMultiplayerManager.OnStartedSyncingTimestamp -= OnStartedSyncingTimestamp;
            HoloKitAppMultiplayerManager.OnStartedSyncingPose -= OnStartedSyncingPose;
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

        private void OnLocalClientConnected()
        {
            _text.text = $"Connected to nearby\nhost {_peerName}";
        }

        private void OnStartedSyncingTimestamp()
        {
            _text.text = $"Syncing timestamp with\nnearby host {_peerName}...";
        }

        private void OnStartedSyncingPose()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnExitReality?.Invoke();
        }
    }
}
