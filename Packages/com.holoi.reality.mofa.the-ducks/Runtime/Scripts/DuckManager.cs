// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.MOFA.TheDucks
{
    public class DuckManager : NetworkBehaviour
    {
        [SerializeField] private Duck _duckPrefab;

        /// <summary>
        /// The position offset from the center eye to the spawn position of the newly spawned duck.
        /// </summary>
        private Vector3 _duckSpawnPositionOffset = new(0f, 0f, 0.3f);

        private Quaternion _duckSpawnRotationOffset = Quaternion.Euler(-45f, 0f, 0f);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitAppUIEventManager.OnStarUITriggered += OnStarUITriggered_Host;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitAppUIEventManager.OnStarUITriggered -= OnStarUITriggered_Host;
            }
        }

        /// <summary>
        /// Spawn the duck across the network.
        /// </summary>
        /// <param name="position">Initial position of the duck</param>
        /// <param name="rotation">Initial rotation of the duck</param>
        public void SpawnDuck(Vector3 position, Quaternion rotation)
        {
            var duckInstance = Instantiate(_duckPrefab, position, rotation);
            duckInstance.GetComponent<NetworkObject>().Spawn();
        }

        private void OnStarUITriggered_Host()
        {
            Transform centerEyePose = HoloKitCameraManager.Instance.CenterEyePose;
            Vector3 position = centerEyePose.position + centerEyePose.rotation * _duckSpawnPositionOffset;
            Quaternion rotation =  centerEyePose.rotation * _duckSpawnRotationOffset;

            SpawnDuck(position, rotation);
        }
    }
}
