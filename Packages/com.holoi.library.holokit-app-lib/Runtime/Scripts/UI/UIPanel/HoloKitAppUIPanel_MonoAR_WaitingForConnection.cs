// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using Unity.Netcode;
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

        private bool _leaving;

        private void Start()
        {
            _transport = NetworkFrameworkTransport.Instance;
            _transport.OnHostFound += OnBrowserFoundPeer;
            _transport.OnHostLost += OnBrowserLostPeer;
            _transport.OnConnectingToHost += OnConnectingWithPeer;
            _transport.OnLocalNetworkDenied += OnLocalNetworkDenied;
            NetworkManager.Singleton.OnClientStopped += OnClientStopped;

            // Start browsing manually
            _transport.StartBrowsing();
            if (_transport.IsLocalNetworkDenied)
                OnLocalNetworkDenied();
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
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

        // A connection attempt that ends before the client joins (the host rejected it, the handshake timed out, or
        // the host left first) stops the client, and nothing started it again: the panel stayed on "Connecting" for
        // good. Search again instead, as leaving and entering the reality would.
        private void OnClientStopped(bool wasHost)
        {
            if (wasHost || _leaving || !isActiveAndEnabled)
                return;
            StartCoroutine(SearchAgain());
        }

        private IEnumerator SearchAgain()
        {
            _text.text = "Searching for a nearby\nhost device...";
            yield return new WaitForSecondsRealtime(1f);
            while (NetworkManager.Singleton != null && NetworkManager.Singleton.ShutdownInProgress)
                yield return null;
            if (_leaving || NetworkManager.Singleton == null)
                yield break;
            HoloKitApp.Instance.RestartClient();
            _transport.StartBrowsing();
        }

        private void Update()
        {
            // Between a failed attempt and the next one the multiplayer manager is gone (see SearchAgain).
            var multiplayerManager = HoloKitApp.Instance.MultiplayerManager;
            if (multiplayerManager != null && multiplayerManager.IsSpawned)
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
            _leaving = true;
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.Shutdown();
        }
    }
}
