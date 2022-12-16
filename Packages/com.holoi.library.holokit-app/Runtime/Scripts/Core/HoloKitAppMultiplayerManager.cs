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

        /// <summary>
        /// The dictionary keeps the refernces of all currently connected players.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _connectedPlayers = new();

        /// <summary>
        /// This event is only called on the server side, which indicates one of the
        /// connected players update its status.
        /// </summary>
        public static event Action OnConnectedPlayerListUpdated;

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
            base.OnNetworkSpawn();
            // Set the reference
            HoloKitApp.Instance.SetMultiplayerManager(this);
            // Spawn the local player
            SpawnPlayerServerRpc();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong senderClientId = serverRpcParams.Receive.SenderClientId;
            // Spawn the player
            var playerInstance = Instantiate(_holokitAppPlayerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(senderClientId);
        }

        /// <summary>
        /// This function is called when a new player joins the network.
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

            // Call the spawn event
            if (HoloKitApp.Instance.IsMaster)
            {
                OnConnectedPlayerListUpdated?.Invoke();
                if (playerClientId == NetworkManager.LocalClientId)
                {
                    player.SyncPose = true;
                }
            }
            else
            {
                if (playerClientId == NetworkManager.LocalClientId)
                {
                    OnLocalPlayerConnected?.Invoke();
                    // Immediately start sync process when connected
                    StartSyncProcess();
                }
            }
        }

        /// <summary>
        /// This function is called when a player leaves the network.
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(HoloKitAppPlayer player)
        {
            ulong playerClientId = player.OwnerClientId;
            if (_connectedPlayers.ContainsKey(playerClientId))
            {
                _connectedPlayers.Remove(playerClientId);
            }

            // Call the despawn event
            if (HoloKitApp.Instance.IsMaster)
            {
                OnConnectedPlayerListUpdated?.Invoke();
            }
            else
            {
                if (playerClientId == NetworkManager.LocalClientId)
                {
                    OnLocalPlayerDisconnected?.Invoke();
                }
            }
        }

        public HoloKitAppPlayer GetLocalPlayer()
        {
            var localPlayerClientId = NetworkManager.LocalClientId;
            if (_connectedPlayers.ContainsKey(localPlayerClientId))
                return _connectedPlayers[localPlayerClientId];
            else
                return null;
        }

        public HoloKitAppPlayer GetMasterPlayer()
        {
            var masterPlayerClientId = NetworkManager.ServerClientId;
            if (_connectedPlayers.ContainsKey(masterPlayerClientId))
                return _connectedPlayers[masterPlayerClientId];
            else
                return null;
        }
    }
}
