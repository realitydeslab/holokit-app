using System;
using System.Collections.Generic;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        public Dictionary<ulong, HoloKitAppPlayer> PlayerDict => _playerDict;

        public ICollection<HoloKitAppPlayer> PlayerList => _playerDict.Values;

        public HoloKitAppPlayer HostPlayer => _playerDict[0];

        public HoloKitAppPlayer LocalPlayer => _playerDict[NetworkManager.LocalClientId];

        public bool ShowPoseVisualizers
        {
            get => _showPoseVisualizers;
            set
            {
                _showPoseVisualizers = value;
            }
        }

        /// <summary>
        /// If this value is set to true, the pose visualizers of all player with checked status will be shown.
        /// </summary>
        private bool _showPoseVisualizers = true;

        /// <summary>
        /// The dictionary which contains all connected player's PlayerObject.
        /// We need this dict because Netcode's default ConnectedClients is not
        /// available on client.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _playerDict = new();

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player has connected to the network.
        /// </summary>
        public static event Action OnLocalPlayerConnected;

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player has disconnected from the network.
        /// </summary>
        public static event Action OnLocalPlayerDisconnected;

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player starts to sync timestamp.
        /// </summary>
        public static event Action OnLocalPlayerSyncingTimestamp;

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player starts to sync pose (via scanning QRCode).
        /// </summary>
        public static event Action OnLocalPlayerSyncingPose;

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player has successfully synced.
        /// </summary>
        public static event Action OnLocalPlayerSynced;

        /// <summary>
        /// This event is only called on the client side, which indicates the local
        /// player has checked the alignment marker.
        /// </summary>
        public static event Action OnLocalPlayerChecked;

        public override void OnNetworkSpawn()
        {
            OnLocalPlayerConnected?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            OnLocalPlayerDisconnected?.Invoke();
        }

        private void Update()
        {
            foreach (var player in PlayerList)
            {
                if (player.IsLocalPlayer) continue;

                if (player.Status.Value == HoloKitAppPlayerStatus.Checked)
                    player.ShowPoseVisualizer = _showPoseVisualizers;
                else
                    player.ShowPoseVisualizer = false;
            }
        }

        private void FixedUpdate()
        {
            // In this phase, client constantly request timestamp from the server.
            if (HoloKitUtils.IsRuntime && IsSpawned && LocalPlayerStatus == HoloKitAppPlayerStatus.SyncingTimestamp)
                OnRequestTimestampServerRpc(HoloKitARSessionControllerAPI.GetSystemUptime());
        }

        /// <summary>
        /// This function is called when a player joins the network.
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerJoined(HoloKitAppPlayer player)
        {
            _playerDict.Add(player.OwnerClientId, player);
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
