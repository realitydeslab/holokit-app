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
        /// <summary>
        /// User-assigned device name of the iOS device.
        /// </summary>
        public NetworkVariable<FixedString64Bytes> Name = new("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<HoloKitAppPlayerType> Type = new(HoloKitAppPlayerType.Player, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<HoloKitAppPlayerStatus> Status = new(HoloKitAppPlayerStatus.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        /// <summary>
        /// When this value is true, the pose of this player is synced to all clients in realtime.
        /// </summary>
        public NetworkVariable<bool> SyncPose = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            SyncPose.OnValueChanged += OnSyncPoseValueChanged;
            // Initialize name and type by the owner
            if (IsOwner)
            {
                Name.Value = new FixedString64Bytes(SystemInfo.deviceName);
                Type.Value = HoloKitApp.Instance.LocalPlayerType;
                if (IsServer)
                {
                    // The host syncs its pose by default
                    SyncPose.Value = true;
                }
                else
                {
                    if (HoloKitUtils.IsRuntime)
                    {
                        // Start syncing the timestamp
                        Status.Value = HoloKitAppPlayerStatus.SyncingTimestamp;
                        HoloKitApp.Instance.MultiplayerManager.LocalPlayerStatus = HoloKitAppPlayerStatus.SyncingTimestamp;
                    }
                    else
                    {
                        // We do not need to sync in editor mode
                        Status.Value = HoloKitAppPlayerStatus.Checked;
                    }
                }
            }
            HoloKitApp.Instance.MultiplayerManager.OnPlayerJoined(this);

            Debug.Log($"[HoloKitAppPlayer] OnNetworkSpawn with ownership {OwnerClientId}");
        }

        public override void OnNetworkDespawn()
        {
            SyncPose.OnValueChanged -= OnSyncPoseValueChanged;
            HoloKitApp.Instance.MultiplayerManager.OnPlayerLeft(this);
        }

        private void OnSyncPoseValueChanged(bool oldValue, bool newValue)
        {
            if (IsOwner)
            {
                if (newValue)
                    SetParentConstraintEnabled(true);
                else
                    SetParentConstraintEnabled(false);
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
