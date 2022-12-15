using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.HoloKitApp
{
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        /// <summary>
        /// The HoloKitAppPlayer prefab which we will spawn for each joined player.
        /// </summary>
        [SerializeField] private HoloKitAppPlayer _holokitAppPlayerPrefab;

        public Dictionary<ulong, HoloKitAppPlayer> ConnectedPlayers => _connectedPlayers;

        public ICollection<HoloKitAppPlayer> ConnectedPlayerList => _connectedPlayers.Values;

        public HoloKitAppPlayer LocalPlayer => _localPlayer;

        /// <summary>
        /// The dictionary keeps the refernces of all currently connected players.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _connectedPlayers = new();

        /// <summary>
        /// Keep a reference of the local player.
        /// </summary>
        private HoloKitAppPlayer _localPlayer;

        public static event Action OnConnectedPlayerListUpdated;

        private void Awake()
        {
            
        }

        private void Start()
        {
            HoloKitAppPlayer.OnPlayerSpawned += OnPlayerDataUpdated;
            HoloKitAppPlayer.OnPlayerDespawned += OnPlayerDataUpdated;
            HoloKitAppPlayer.OnPlayerSyncStatusUpdated += OnPlayerDataUpdated;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Set the reference
            HoloKitApp.Instance.SetMultiplayerManager(this);
            // Spawn the local player
            string userAssignedDeviceName = SystemInfo.deviceName;
            var type = HoloKitApp.Instance.LocalPlayerType;
            SpawnPlayerServerRpc(userAssignedDeviceName, type);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            HoloKitAppPlayer.OnPlayerSpawned -= OnPlayerDataUpdated;
            HoloKitAppPlayer.OnPlayerDespawned -= OnPlayerDataUpdated;
            HoloKitAppPlayer.OnPlayerSyncStatusUpdated -= OnPlayerDataUpdated;
        }

        [ServerRpc]
        private void SpawnPlayerServerRpc(string name, HoloKitAppPlayerType type, ServerRpcParams serverRpcParams = default)
        {
            ulong senderClientId = serverRpcParams.Receive.SenderClientId;
            bool isMaster = senderClientId == NetworkManager.ServerClientId;
            // Spawn the player
            var playerInstance = Instantiate(_holokitAppPlayerPrefab);
            playerInstance.Name = name;
            playerInstance.IsMaster = senderClientId == NetworkManager.ServerClientId;
            playerInstance.Type = type;
            playerInstance.SyncStatus = isMaster ? HoloKitAppPlayerSyncStatus.Synced : HoloKitAppPlayerSyncStatus.None;
            // Only the host device syncs its pose at the beginning
            playerInstance.SyncPose = isMaster;
            playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(senderClientId);
        }

        /// <summary>
        /// This function is called when a new player joins.
        /// </summary>
        /// <param name="player">The newly joined player</param>
        public void SetPlayer(HoloKitAppPlayer player)
        {
            ulong playerClientId = player.OwnerClientId;
            if (_connectedPlayers.ContainsKey(playerClientId))
            {
                _connectedPlayers[playerClientId] = player;
            }
            else
            {
                _connectedPlayers.Add(playerClientId, player);
            }
            player.transform.SetParent(this.transform);

            // This is the local player
            if (playerClientId == OwnerClientId)
            {
                _localPlayer = player;
                if (!IsServer)
                {
                    StartSpatialAnchorSyncProcess();
                }
            }
        }

        public HoloKitAppPlayer GetServerPlayer()
        {
            return _connectedPlayers[NetworkManager.ServerClientId];
        }

        /// <summary>
        /// This function is called when a joined player leaves.
        /// </summary>
        /// <param name="ownerClientId">The clientId of the left player</param>
        public void RemovePlayer(ulong ownerClientId)
        {
            if (_connectedPlayers.ContainsKey(ownerClientId))
            {
                _connectedPlayers.Remove(ownerClientId);
            }
        }

        /// <summary>
        /// This delegate function is called either when a player is spawned, despawned
        /// or sync status updated.
        /// </summary>
        /// <param name="playerClientId"></param>
        private void OnPlayerDataUpdated(ulong playerClientId)
        {
            OnConnectedPlayerListUpdated?.Invoke();
        }
    }
}
