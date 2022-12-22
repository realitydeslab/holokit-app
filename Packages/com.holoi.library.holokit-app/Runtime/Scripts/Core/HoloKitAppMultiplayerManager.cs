using System.Collections.Generic;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        public HoloKitAppPlayerPoseVisualizer PoseVisualizerPrefab;

        public Dictionary<ulong, HoloKitAppPlayer> PlayerDict => _playerDict;

        public ICollection<HoloKitAppPlayer> PlayerList => _playerDict.Values;

        public HoloKitAppPlayer HostPlayer => _playerDict.ContainsKey(0) ? _playerDict[0] : null;

        public HoloKitAppPlayer LocalPlayer => _playerDict.ContainsKey(NetworkManager.LocalClientId) ? _playerDict[NetworkManager.LocalClientId] : null;

        /// <summary>
        /// This is a local property which controls the visibility of pose visualizers of players.
        /// </summary>
        public bool ShowPoseVisualizers { get; set; }

        /// <summary>
        /// The dictionary which contains all connected player's PlayerObject.
        /// We need this dict because Netcode's default ConnectedClients is not
        /// available on client.
        /// </summary>
        private readonly Dictionary<ulong, HoloKitAppPlayer> _playerDict = new();

        private void Update()
        {
            if (CurrentStatus == HoloKitAppPlayerStatus.SyncingTimestamp)
                OnRequestTimestampServerRpc(HoloKitARSessionControllerAPI.GetSystemUptime());

            // Update pose visualizers' visibility
            foreach (var player in PlayerList)
            {
                if (player.SyncingPose.Value && player.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked)
                {
                    player.ShowPoseVisualizer = ShowPoseVisualizers;
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

            // Start spatial anchor synchronization
            if (!IsServer && player.IsLocalPlayer)
            {
                CurrentStatus = HoloKitAppPlayerStatus.SyncingTimestamp;
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
