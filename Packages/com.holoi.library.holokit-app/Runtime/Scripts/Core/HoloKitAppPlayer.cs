using System;
using UnityEngine;
using UnityEngine.Animations;
using Unity.Netcode;
using Unity.Netcode.Components.ClientAuthority;
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
        Checked = 4,
        // When player disconnected from the network
        Disconnected = 5
    }

    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]
    [RequireComponent(typeof(ParentConstraint))]
    public class HoloKitAppPlayer : NetworkBehaviour
    {
        /// <summary>
        /// User-assigned device name of the iOS device.
        /// </summary>
        public NetworkVariable<FixedString64Bytes> Name = new("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<HoloKitAppPlayerType> Type = new(HoloKitAppPlayerType.Player, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<HoloKitAppPlayerStatus> Status = new(HoloKitAppPlayerStatus.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public bool ShowPoseVisualizer
        {
            set
            {
                if (value)
                    SpawnPoseVisualizer();
                else
                    DestroyPoseVisualizer();
            }
        }

        private HoloKitAppPlayerPoseVisualizer _poseVisualizer;

        public static event Action<HoloKitAppPlayer> OnPlayerConnected; 

        public static event Action<HoloKitAppPlayer> OnPlayerStatusChanged;

        public static event Action<HoloKitAppPlayer> OnPlayerDisconnected;

        public override void OnNetworkSpawn()
        {
            Status.OnValueChanged += OnStatusValueChanged;
            HoloKitApp.Instance.MultiplayerManager.OnPlayerJoined(this);
            // Initialize name and type by the owner
            if (IsOwner)
            {
                Name.Value = new FixedString64Bytes(SystemInfo.deviceName);
                Type.Value = HoloKitApp.Instance.LocalPlayerType;
                // Setup initial status
                if (IsServer)
                {
                    // Set host's status to checked at start
                    Status.Value = HoloKitAppPlayerStatus.Checked;
                }
                else
                {
                    if (HoloKitUtils.IsRuntime)
                        Status.Value = HoloKitAppPlayerStatus.SyncingTimestamp;
                    else
                        Status.Value = HoloKitAppPlayerStatus.Checked;
                }
                // Setup ParentConstraint
                SetupParentConstraint();
            }
            OnPlayerConnected?.Invoke(this);
        }

        public override void OnNetworkDespawn()
        {
            Status.OnValueChanged -= OnStatusValueChanged;
            HoloKitApp.Instance.MultiplayerManager.OnPlayerLeft(this);
            OnPlayerDisconnected?.Invoke(this);
        }

        private void OnStatusValueChanged(HoloKitAppPlayerStatus oldStatus, HoloKitAppPlayerStatus newStatus)
        {
            if (oldStatus == newStatus) return;

            OnPlayerStatusChanged?.Invoke(this);
        }

        private void SetupParentConstraint()
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

        private void SpawnPoseVisualizer()
        {
            if (_poseVisualizer == null)
            {
                var poseVisualizerPrefab = HoloKitApp.Instance.MultiplayerManager.PoseVisualizerPrefab;
                _poseVisualizer = Instantiate(poseVisualizerPrefab);
                _poseVisualizer.Player = this;
            }
        }

        private void DestroyPoseVisualizer()
        {
            if (_poseVisualizer != null)
            {
                Destroy(_poseVisualizer);
            }
        }

        /// <summary>
        /// Get the distance of this player to the local device.
        /// </summary>
        /// <returns>Distance in meters</returns>
        public float GetDist()
        {
            return Vector3.Distance(HoloKitCamera.Instance.CenterEyePose.position, transform.position);
        }
    }
}
