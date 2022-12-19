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

        private string _hostName;

        private void Start()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer += OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer += OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer += OnConnectingWithPeer;
            HoloKitAppPlayer.OnPlayerStatusChanged += OnPlayerStatusChanged;

            // Start browsing manually
            MultipeerConnectivityTransport.StartBrowsing();
        }

        private void OnDestroy()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
            HoloKitAppPlayer.OnPlayerStatusChanged -= OnPlayerStatusChanged;
        }

        private void OnBrowserFoundPeer(string hostName)
        {
            _hostName = hostName;
            _text.text = $"Found nearby host\n{hostName}...";
        }

        private void OnBrowserLostPeer(string hostName)
        {
            _text.text = $"Lost nearby host\n{hostName}";
        }

        private void OnConnectingWithPeer(string hostName)
        {
            _hostName = hostName;
            _text.text = $"Connecting to nearby\nhost {hostName}...";
        }

        private void OnPlayerStatusChanged(HoloKitAppPlayer player)
        {
            if (player.IsLocalPlayer)
            {
                switch (player.Status.Value)
                {
                    case HoloKitAppPlayerStatus.None:
                        _text.text = $"Connected with nearby\nhost {_hostName}";
                        break;
                    case HoloKitAppPlayerStatus.SyncingTimestamp:
                        _text.text = $"Syncing timestamp with\nnearby host {_hostName}...";
                        break;
                    case HoloKitAppPlayerStatus.SyncingPose:
                        HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                        HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
                        break;
                    case HoloKitAppPlayerStatus.Synced:
                        break;
                    case HoloKitAppPlayerStatus.Checked:
                        break;
                    case HoloKitAppPlayerStatus.Disconnected:
                        _text.text = $"Disconnected from nearby\nhost {_hostName}";
                        break;
                }
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.Shutdown();
        }
    }
}
