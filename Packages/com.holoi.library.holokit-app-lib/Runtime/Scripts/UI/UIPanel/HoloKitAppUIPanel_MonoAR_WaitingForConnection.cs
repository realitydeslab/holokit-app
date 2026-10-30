// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Netcode.Transports.NetworkFramework;
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIPanel_MonoAR_WaitingForConnection : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_WaitingForConnection";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _text;

        private string _hostName;

        private NetworkFrameworkTransport _transport;

        private void Start()
        {
            _transport = NetworkFrameworkTransport.Instance;
            _transport.OnHostFound += OnBrowserFoundPeer;
            _transport.OnHostLost += OnBrowserLostPeer;
            _transport.OnConnectingToHost += OnConnectingWithPeer;
            _transport.OnLocalNetworkDenied += OnLocalNetworkDenied;

            // Start browsing manually
            _transport.StartBrowsing();
            if (_transport.IsLocalNetworkDenied)
                OnLocalNetworkDenied();
        }

        private void OnDestroy()
        {
            // The transport goes away with the NetworkManager when a reality shuts down.
            if (_transport == null)
                return;
            _transport.OnHostFound -= OnBrowserFoundPeer;
            _transport.OnHostLost -= OnBrowserLostPeer;
            _transport.OnConnectingToHost -= OnConnectingWithPeer;
            _transport.OnLocalNetworkDenied -= OnLocalNetworkDenied;
        }

        // Without Local Network access nothing nearby can be found, and iOS shows no error of its own.
        private void OnLocalNetworkDenied()
        {
            _text.text = $"Turn on Local Network for {Application.productName}\nin Settings > Privacy & Security";
        }

        private void OnBrowserFoundPeer(int hostKey, string hostName)
        {
            _hostName = hostName;
            _text.text = $"Found nearby host\n{hostName}...";
        }

        private void OnBrowserLostPeer(int hostKey, string hostName)
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
