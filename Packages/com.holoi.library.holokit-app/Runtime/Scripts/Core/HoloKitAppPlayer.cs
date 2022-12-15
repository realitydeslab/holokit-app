using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components.ClientAuthority;
using Unity.Collections;

namespace Holoi.Library.HoloKitApp
{
    public enum HoloKitAppPlayerType
    {
        Player = 0,
        Spectator = 1,
        Puppeteer = 2
    }

    public enum HoloKitAppPlayerSyncStatus
    {
        None = 0,
        SyncingTimestamp = 1,
        SyncingPose = 2,
        Synced = 3
    }

    public class HoloKitAppPlayer : NetworkBehaviour
    {
        public string Name
        {
            get => _name.Value.ToString();
            set
            {
                if (!IsSpawned || (IsSpawned && IsServer))
                {
                    _name.Value = new FixedString64Bytes(value);
                }
                else
                {
                    Debug.Log("Only the master can set network variable: _name");
                }
            }
        }

        public bool IsMaster
        {
            get => _isMaster.Value;
            set
            {
                if (!IsSpawned || (IsSpawned && IsServer))
                {
                    _isMaster.Value = value;
                }
                else
                {
                    Debug.Log("Only the master can set network variable: _isMaster");
                }
            }
        }

        public HoloKitAppPlayerType Type
        {
            get => _type.Value;
            set
            {
                if (!IsSpawned || (IsSpawned && IsServer))
                {
                    _type.Value = value;
                }
                else
                {
                    Debug.Log("Only the master can set network variable: _type");
                }
            }
        }

        public HoloKitAppPlayerSyncStatus SyncStatus
        {
            get => _syncStatus.Value;
            set
            {
                if (!IsSpawned || (IsSpawned && IsOwner))
                {
                    _syncStatus.Value = value;
                }
                else
                {
                    Debug.Log("Only the owner can set network variable: _syncStatus");
                }
            }
        }

        public bool SyncPose
        {
            get => _syncPose.Value;
            set
            {
                if (!IsSpawned || (IsSpawned && IsServer))
                {
                    _syncPose.Value = value;
                }
                else
                {
                    Debug.Log("Only the master can set network variable: _syncPose");
                }
            }
        }

        /// <summary>
        /// This is the device name of iPhone.
        /// </summary>
        private readonly NetworkVariable<FixedString64Bytes> _name = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// If this player is the host?
        /// </summary>
        private readonly NetworkVariable<bool> _isMaster = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<HoloKitAppPlayerType> _type = new(HoloKitAppPlayerType.Player, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<HoloKitAppPlayerSyncStatus> _syncStatus = new(HoloKitAppPlayerSyncStatus.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        /// <summary>
        /// When this value is true, the pose of this player is synced to all clients in realtime.
        /// </summary>
        private readonly NetworkVariable<bool> _syncPose = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// Keep a reference of the ClientNetworkTransform component so we can enable and disable it.
        /// </summary>
        private ClientNetworkTransform _clientNetworkTransform;

        /// <summary>
        /// This event is called when a new player joins the network.
        /// The parameter is the player client id.
        /// </summary>
        public static event Action<ulong> OnPlayerSpawned;

        /// <summary>
        /// This event is called when a player disconnects from the network.
        /// The parameter is the player client id.
        /// </summary>
        public static event Action<ulong> OnPlayerDespawned;

        /// <summary>
        /// This event is called when a player's sync status updates.
        /// The parameter is the player client id.
        /// </summary>
        public static event Action<ulong> OnPlayerSyncStatusUpdated;

        private void Awake()
        {
            _clientNetworkTransform = GetComponent<ClientNetworkTransform>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Register network variable value change callbacks
            _syncStatus.OnValueChanged += OnSyncStatusValueChanged;
            _syncPose.OnValueChanged += OnSyncPoseValueChanged;
            // Set the reference
            HoloKitApp.Instance.MultiplayerManager.SetPlayer(this);
            // Call the spawn event
            OnPlayerSpawned?.Invoke(OwnerClientId);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            // Unregister network variable value change callbacks
            _syncStatus.OnValueChanged -= OnSyncStatusValueChanged;
            _syncPose.OnValueChanged -= OnSyncPoseValueChanged;
            // Remove the reference
            HoloKitApp.Instance.MultiplayerManager.RemovePlayer(OwnerClientId);
            // Call the despawn event
            OnPlayerDespawned?.Invoke(OwnerClientId);
        }

        private void OnSyncStatusValueChanged(HoloKitAppPlayerSyncStatus oldStatus, HoloKitAppPlayerSyncStatus newStatus)
        {
            if (oldStatus != newStatus)
            {
                OnPlayerSyncStatusUpdated?.Invoke(OwnerClientId);
            }
        }

        private void OnSyncPoseValueChanged(bool oldValue, bool newValue)
        {
            if (IsOwner)
            {
                if (!oldValue && newValue)
                {
                    _clientNetworkTransform.enabled = true;
                }
                else if (oldValue && !newValue)
                {
                    _clientNetworkTransform.enabled = false;
                }
            }
        }

        /// <summary>
        /// Get the distance of this player to the local device.
        /// </summary>
        /// <returns>Distance in meters</returns>
        public float GetDist()
        {
            return 0f;
        }
    }
}
