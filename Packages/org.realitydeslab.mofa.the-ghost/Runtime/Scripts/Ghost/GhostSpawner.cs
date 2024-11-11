// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;

namespace RealityDesignLab.MOFA.TheGhost
{
    public class GhostSpawner : NetworkBehaviour
    {
        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementManager;

        [Header("Ghost")]
        [SerializeField] private Ghost _ghostPrefab;

        private Ghost _ghost;

        public static event Action OnGhostSpawned;

        private void Start()
        {
            if (HoloKitApp.Instance.IsPuppeteer)
                HoloKitAppUIEventManager.OnStarUITriggered += OnUITriggered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (HoloKitApp.Instance.IsPuppeteer)
                HoloKitAppUIEventManager.OnStarUITriggered -= OnUITriggered;
        }

        public override void OnNetworkSpawn()
        {
            if (HoloKitApp.Instance.IsPuppeteer)
            {
                StartGhostPlacement();
            }
            else
            {
                _arPlacementManager.OnDisabledFunc();
            }
        }

        /// <summary>
        /// Call this function to 
        /// </summary>
        private void StartGhostPlacement()
        {
            _arPlaneManager.enabled = true;
            _arRaycastManager.enabled = true;
            _arPlacementManager.IsActive = true;
        }

        private void EndGhostPlacement()
        {
            _arPlaneManager.enabled = false;
            _arRaycastManager.enabled = false;
            _arPlacementManager.OnPlacedFunc();
        }

        private void OnUITriggered()
        {
            if (_ghost == null && _arPlacementManager.IsValid)
            {
                Transform hitPoint = _arPlacementManager.HitPoint;
                SpawnGhost(hitPoint.position, hitPoint.rotation);
                EndGhostPlacement();
            }
        }

        private void SpawnGhost(Vector3 position, Quaternion rotation)
        {
            _ghost = Instantiate(_ghostPrefab, position, rotation);
            _ghost.GetComponent<NetworkObject>().Spawn();
            OnGhostSpawned?.Invoke();
        }
    }
}
