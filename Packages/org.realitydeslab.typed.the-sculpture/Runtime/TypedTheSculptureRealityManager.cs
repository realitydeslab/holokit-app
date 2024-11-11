// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.ARUX;
using HoloKit;

namespace RealityDesignLab.Typed.TheSculpture
{
    public class TypedTheSculptureRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementIndicator;

        [SerializeField] private HoverableStartButton _hoverableStartButton;

        [Header("Hand")]
        public HoverObject HostHandPose;

        [SerializeField] private GameObject _hostHandVisual;

        [Header("The Sculpture")]
        [SerializeField] private GameObject _sculpturePrefab;

        private readonly NetworkVariable<bool> _isHostHandValid = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = UnityEngine.XR.ARSubsystems.EnvironmentDepthMode.Fastest;
                _arRaycastManager.enabled = true;
                _arPlaneManager.enabled = true;
                HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
                HoloKitHandTracker.Instance.IsActive = true;
                _hostHandVisual.SetActive(false);
                _arPlacementIndicator.IsActive = true;

                if (HoloKitUtils.IsEditor)
                {
                    _hostHandVisual.SetActive(true);
                }
            }
            else
            {
                _arPlaneManager.enabled = true;
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_hoverableStartButton.gameObject);
                Destroy(HostHandPose.GetComponent<FollowTargetController>());
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isHostHandValid.OnValueChanged += OnIsHostHandValidValueChanged;

            if (!IsServer)
                HoloKitApp.Instance.ARSessionManager.SetEnvironmentOcclusionEnabled(false);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isHostHandValid.OnValueChanged -= OnIsHostHandValidValueChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.OnHandValidityChanged -= OnHandValidityChanged;
            }
        }

        public void SpawnSculpture()
        {
            Debug.Log("Spawn The Sculpture");
            var hitPoint = _arPlacementIndicator.HitPoint;
            var sculpture = Instantiate(_sculpturePrefab, hitPoint.position, hitPoint.rotation);
            sculpture.GetComponent<NetworkObject>().Spawn();

            _arRaycastManager.enabled = false;
            _arPlacementIndicator.OnPlacedFunc();
            _hoverableStartButton.OnDeath();
        }

        private void OnHandValidityChanged(bool isValid)
        {
            _isHostHandValid.Value = isValid;
        }

        private void OnIsHostHandValidValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                HostHandPose.IsActive = true;
                _hostHandVisual.SetActive(true);
                return;
            }

            if (oldValue && !newValue)
            {
                HostHandPose.IsActive = false;
                _hostHandVisual.SetActive(false);
                return;
            }
        }
    }
}