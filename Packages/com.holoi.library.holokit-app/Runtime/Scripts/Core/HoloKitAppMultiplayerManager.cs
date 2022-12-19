using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        public Dictionary<ulong, HoloKitAppPlayer> PlayerDict => _playerDict;

        public ICollection<HoloKitAppPlayer> PlayerList => _playerDict.Values;

        public HoloKitAppPlayer HostPlayer => _playerDict.ContainsKey(0) ? _playerDict[0] : null;

        public HoloKitAppPlayer LocalPlayer => _playerDict.ContainsKey(NetworkManager.LocalClientId) ? _playerDict[NetworkManager.LocalClientId] : null;

        /// <summary>
        /// The dictionary which contains all connected player's PlayerObject.
        /// We need this dict because Netcode's default ConnectedClients is not
        /// available on client.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _playerDict = new();

        private void FixedUpdate()
        {
            if (!IsSpawned) return;
            var localPlayer = LocalPlayer;
            if (localPlayer == null) return;
            if (localPlayer.Status.Value == HoloKitAppPlayerStatus.SyncingTimestamp)
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
