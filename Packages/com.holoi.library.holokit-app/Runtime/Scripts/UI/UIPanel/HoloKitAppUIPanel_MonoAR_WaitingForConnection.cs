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

            // Start browsing manually
            MultipeerConnectivityTransport.StartBrowsing();
        }

        private void OnDestroy()
        {
            MultipeerConnectivityTransport.OnBrowserFoundPeer -= OnBrowserFoundPeer;
            MultipeerConnectivityTransport.OnBrowserLostPeer -= OnBrowserLostPeer;
            MultipeerConnectivityTransport.OnConnectingWithPeer -= OnConnectingWithPeer;
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

        private void Update()
        {
            if (HoloKitApp.Instance.MultiplayerManager.IsSpawned)
            {
                var localPlayer = HoloKitApp.Instance.MultiplayerManager.LocalPlayer;
                if (localPlayer != null)
                {
                    if (localPlayer.PlayerStatus.Value == HoloKitAppPlayerStatus.None)
                    {
                        _text.text = $"Connected with nearby\nhost {_hostName}";
                        return;
                    }

                    if (localPlayer.PlayerStatus.Value == HoloKitAppPlayerStatus.SyncingTimestamp)
                    {
                        _text.text = $"Syncing timestamp with\nnearby host {_hostName}...";
                        return;
                    }

                    if (localPlayer.PlayerStatus.Value == HoloKitAppPlayerStatus.SyncingPose)
                    {
                        Debug.Log("666666");
                        HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                        HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
                        return;
                    }
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
