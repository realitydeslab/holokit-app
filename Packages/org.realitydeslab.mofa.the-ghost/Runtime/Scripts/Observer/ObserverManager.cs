// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib.UI;
using Holoi.Library.HoloKitAppLib;
using HoloKit;

namespace RealityDesignLab.MOFA.TheGhost
{
    public class ObserverManager : NetworkBehaviour
    {
        [SerializeField] private DetectionWave _detectionWavePrefab;

        /// <summary>
        /// The offset from the center eye position to the wave.
        /// </summary>
        [SerializeField] private Vector3 _waveSpawnOffset;

        private void Awake()
        {
            // Only the observer play should register this event
            if (HoloKitApp.Instance.IsPlayer && HoloKitApp.Instance.LocalPlayerTypeSubindex == 0)
            {
                HoloKitAppUIEventManager.OnStarUITriggered += OnUITriggered;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (HoloKitApp.Instance.IsPlayer && HoloKitApp.Instance.LocalPlayerTypeSubindex == 0)
            {
                HoloKitAppUIEventManager.OnStarUITriggered -= OnUITriggered;
            }
        }

        private void OnUITriggered()
        {
            Transform centerEyePose = HoloKitCameraManager.Instance.CenterEyePose;
            SpawnDetectionWaveServerRpc(centerEyePose.position, centerEyePose.rotation);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDetectionWaveServerRpc(Vector3 centerEyePosition, Quaternion centerEyeRotation, ServerRpcParams serverRpcParams = default)
        {
            Vector3 position = centerEyePosition + centerEyeRotation * _waveSpawnOffset;
            Quaternion rotation = centerEyeRotation;

            // Spawn the detection wave
            var waveInstance = Instantiate(_detectionWavePrefab, position, rotation);
            waveInstance.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        }
    }
}
