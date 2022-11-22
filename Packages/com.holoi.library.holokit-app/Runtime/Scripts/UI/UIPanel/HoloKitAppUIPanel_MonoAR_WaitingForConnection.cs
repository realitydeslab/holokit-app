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

        private void Start()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer += OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer += OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer += OnConnectingWithPeer;
            HoloKitAppMultiplayerManager.OnLocalClientConnected += OnLocalClientConnected;
        }

        private void OnDestroy()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
            HoloKitAppMultiplayerManager.OnLocalClientConnected -= OnLocalClientConnected;
        }

        private void OnLocalClientConnected()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitAppUIEventManager.OnExitReality?.Invoke();
        }

        private void OnBrowserFoundPeer(string peerName)
        {
            _text.text = $"Found nearby device\n{peerName}...";
        }

        private void OnBrowserLostPeer(string peerName)
        {
            _text.text = $"Lost nearby device\n{peerName}";
        }

        private void OnConnectingWithPeer(string peerName)
        {
            _text.text = $"Connecting to nearby\ndevice {peerName}...";
        }
    }
}
