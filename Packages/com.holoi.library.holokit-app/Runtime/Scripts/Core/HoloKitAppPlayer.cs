using UnityEngine;
using UnityEngine.Animations;
using Unity.Netcode;
using Unity.Collections;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public enum HoloKitAppPlayerType
    {
        Player = 0,
        Spectator = 1,
        Puppeteer = 2
    }

    public enum HoloKitAppPlayerStatus
    {
        None = 0,
        // When the player trying to sync timestamp to a nearby master
        SyncingTimestamp = 1,
        // When the player trying to sync pose to a nearby master by scanning QRCode
        SyncingPose = 2,
        // When the player synced to a nearby master
        Synced = 3,
        // When the player conformed the sync result
        Checked = 4
    }

    public class HoloKitAppPlayer : NetworkBehaviour
    {
        public string Name
        {
            get => _name.Value.ToString();
            set
            {
                if (IsOwner)
                {
                    _name.Value = new FixedString64Bytes(value);
                }
                else
                {
                    Debug.Log("Only the owner can set network variable: _name");
                }
            }
        }

        public HoloKitAppPlayerType Type
        {
            get => _type.Value;
            set
            {
                if (IsOwner)
                {
                    _type.Value = value;
                }
                else
                {
                    Debug.Log("Only the owner can set network variable: _type");
                }
            }
        }

        public HoloKitAppPlayerStatus Status
        {
            get => _status.Value;
            set
            {
                if (IsServer)
                {
                    _status.Value = value;
                }
                else
                {
                    Debug.Log("Only the server can set network variable: _status");
                }
            }
        }

        public bool SyncPose
        {
            get => _syncPose.Value;
            set
            {
                if (IsServer)
                {
                    _syncPose.Value = value;
                }
                else
                {
                    Debug.Log("Only the server can set network variable: _syncPose");
                }
            }
        }

        /// <summary>
        /// This is the device name of iPhone.
        /// </summary>
        private readonly NetworkVariable<FixedString64Bytes> _name = new("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<HoloKitAppPlayerType> _type = new(HoloKitAppPlayerType.Player, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<HoloKitAppPlayerStatus> _status = new(HoloKitAppPlayerStatus.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// When this value is true, the pose of this player is synced to all clients in realtime.
        /// </summary>
        private readonly NetworkVariable<bool> _syncPose = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Register network variable value change callbacks
            _syncPose.OnValueChanged += OnPlayerSyncPoseValueChanged;
            // Initialize name and type
            if (IsOwner)
            {
                _name.Value = new FixedString64Bytes(SystemInfo.deviceName);
                _type.Value = HoloKitApp.Instance.LocalPlayerType;
            }
            // Set the reference
            HoloKitApp.Instance.MultiplayerManager.SetPlayer(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            // Unregister network variable value change callbacks
            _syncPose.OnValueChanged -= OnPlayerSyncPoseValueChanged;
            // Remove the reference
            HoloKitApp.Instance.MultiplayerManager.RemovePlayer(this);
        }

        private void OnPlayerSyncPoseValueChanged(bool oldValue, bool newValue)
        {
            if (IsOwner)
            {
                if (!oldValue && newValue)
                {
                    // Turned on
                    SetParentConstraintEnabled(true);
                }
                else if (oldValue && !newValue)
                {
                    // Turned off
                    SetParentConstraintEnabled(false);
                }
            } 
        }

        private void SetParentConstraintEnabled(bool enabled)
        {
            var parentConstraint = GetComponent<ParentConstraint>();
            if (enabled)
            {
                ConstraintSource constraintSource = new();
                constraintSource.sourceTransform = HoloKitCamera.Instance.CenterEyePose;
                constraintSource.weight = 1f;
                parentConstraint.AddSource(constraintSource);
                parentConstraint.weight = 1f;
                parentConstraint.constraintActive = true;
            }
            else
            {
                parentConstraint.constraintActive = false;
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
