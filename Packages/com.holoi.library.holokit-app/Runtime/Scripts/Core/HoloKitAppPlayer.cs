// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

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
        Puppeteer = 1,
        Spectator = 2
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
        public NetworkVariable<FixedString64Bytes> PlayerName = new("Anonymous", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<HoloKitAppPlayerType> PlayerType = new(HoloKitAppPlayerType.Player, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<int> PlayerTypeSubindex = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<HoloKitAppPlayerStatus> PlayerStatus = new(HoloKitAppPlayerStatus.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> SyncingPose = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public bool ShowPoseVisualizer
        {
            get => _poseVisualizer != null;
            set
            {
                if (value)
                    SpawnPoseVisualizer();
                else
                    DestroyPoseVisualizer();
            }
        }
        
        private ParentConstraint _parentConstraint;

        private HoloKitAppPlayerPoseVisualizer _poseVisualizer;

        public override void OnNetworkSpawn()
        {
            SyncingPose.OnValueChanged += OnSyncingPoseValueChanged;

            if (IsOwner)
            {
                InitPlayerInfoServerRpc(SystemInfo.deviceName, HoloKitApp.Instance.LocalPlayerType);
                SetupParentConstraint(false);
            }
            
            HoloKitApp.Instance.MultiplayerManager.OnPlayerJoined(this);
        }

        public override void OnNetworkDespawn()
        {
            SyncingPose.OnValueChanged -= OnSyncingPoseValueChanged;

            HoloKitApp.Instance.MultiplayerManager.OnPlayerLeft(this);
        }

        private void OnSyncingPoseValueChanged(bool oldValue, bool newValue)
        {
            if (IsOwner)
            {
                if (!oldValue && newValue) // Turned on
                {
                    if (_parentConstraint != null)
                        _parentConstraint.constraintActive = true;
                }
                else if (oldValue && !newValue) // Turned off
                {
                    if (_parentConstraint != null)
                        _parentConstraint.constraintActive = false;
                }
            }
        }

        [ServerRpc]
        private void InitPlayerInfoServerRpc(string playerName, HoloKitAppPlayerType playerType)
        {
            PlayerName.Value = new FixedString64Bytes(playerName);
            PlayerType.Value = playerType;
        }

        private void SetupParentConstraint(bool syncPose)
        {
            _parentConstraint = GetComponent<ParentConstraint>();
            if (_parentConstraint != null)
            {
                ConstraintSource constraintSource = new();
                constraintSource.sourceTransform = HoloKitCamera.Instance.CenterEyePose;
                constraintSource.weight = 1f;
                _parentConstraint.AddSource(constraintSource);
                _parentConstraint.weight = 1f;
                _parentConstraint.constraintActive = syncPose;
            }
        }

        [ServerRpc]
        public void UpdatePlayerStatusServerRpc(HoloKitAppPlayerStatus newStatus)
        {
            PlayerStatus.Value = newStatus;
        }

        [ServerRpc]
        public void SetSyncingPoseServerRpc(bool syncing)
        {
            SyncingPose.Value = syncing;
        }

        public void SpawnPoseVisualizer()
        {
            if (_poseVisualizer == null)
            {
                // We don't spawn pose visualizer for the local player
                if (!IsLocalPlayer)
                {
                    var poseVisualizerPrefab = HoloKitApp.Instance.MultiplayerManager.PoseVisualizerPrefab;
                    _poseVisualizer = Instantiate(poseVisualizerPrefab);
                    _poseVisualizer.Player = this;
                }
            }
        }

        public void DestroyPoseVisualizer()
        {
            if (_poseVisualizer != null)
            {
                Destroy(_poseVisualizer.gameObject);
            }
        }

        /// <summary>
        /// Get the distance of this player to the local player.
        /// </summary>
        /// <returns>Distance in meters</returns>
        public float GetDistanceToLocalPlayer()
        {
            return Vector3.Distance(HoloKitCamera.Instance.CenterEyePose.position, transform.position);
        }
    }
}
