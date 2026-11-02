// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib
{
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        public HoloKitAppPlayerPoseVisualizer PoseVisualizerPrefab;

        public Dictionary<ulong, HoloKitAppPlayer> PlayerDict => _playerDict;

        public ICollection<HoloKitAppPlayer> PlayerList => _playerDict.Values;

        public HoloKitAppPlayer HostPlayer => _playerDict.ContainsKey(0) ? _playerDict[0] : null;

        public HoloKitAppPlayer LocalPlayer
        {
            get
            {
                var networkManager = NetworkManager.Singleton;
                return networkManager != null && _playerDict.TryGetValue(networkManager.LocalClientId, out var player)
                    ? player : null;
            }
        }

        /// <summary>
        /// This is a local property which controls the visibility of pose visualizers of players.
        /// </summary>
        public bool ShowPoseVisualizers { get; set; }

        /// <summary>
        /// The number of players who participated in this reality.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// The dictionary which contains all connected player's PlayerObject.
        /// We need this dict because Netcode's default ConnectedClients is not
        /// available on client.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _playerDict = new();

        // LateUpdate, after coroutines and UI events, so a recording started this frame already renders without them.
        private void LateUpdate()
        {
            // Update pose visualizers' visibility. They help the people in the room and stay out of recordings.
            var recorder = HoloKitApp.Instance != null ? HoloKitApp.Instance.Recorder : null;
            var show = ShowPoseVisualizers && !(recorder != null && recorder.IsRecording);
            foreach (var player in PlayerList)
            {
                if (player.SyncingPose.Value && player.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked)
                {
                    player.ShowPoseVisualizer = show;
                }
                else
                {
                    player.ShowPoseVisualizer = false;
                }
            }
        }

        /// <summary>
        /// This function is called when a player joins the network.
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerJoined(HoloKitAppPlayer player)
        {
            _playerDict.Add(player.OwnerClientId, player);
            PlayerCount++;

            if (player.IsLocalPlayer)
            {
                if (HoloKitApp.Instance.IsHost || HoloKitUtils.IsEditor)
                    CurrentStatus = HoloKitAppPlayerStatus.Checked;
                else
                    CurrentStatus = HoloKitAppPlayerStatus.SyncingTimestamp;

                player.SetSyncingPoseServerRpc(true);
            }
        }

        /// <summary>
        /// This function is called when a player leaves the network.
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerLeft(HoloKitAppPlayer player)
        {
            _playerDict.Remove(player.OwnerClientId);
        }
    }
}
