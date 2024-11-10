// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class NetworkHoloKitHandTracker : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<bool> IsValid = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [SerializeField] private AROcclusionManager _arOcclusionManager;

        public static event Action<bool> OnHostHandValidityChanged;

        private void Start()
        {
            HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;
                _arOcclusionManager.environmentDepthTemporalSmoothingRequested = false;
                HoloKitHandTracker.Instance.IsActive = true;
            }
            else
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
                HoloKitHandTracker.Instance.IsActive = false;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            IsValid.OnValueChanged += OnIsValidValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            IsValid.OnValueChanged -= OnIsValidValueChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            HoloKitHandTracker.OnHandValidityChanged -= OnHandValidityChanged;
        }

        private void OnHandValidityChanged(bool isValid)
        {
            if (IsSpawned && IsServer)
            {
                IsValid.Value = isValid;
            }
        }

        private void OnIsValidValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                // Turned on
                OnHostHandValidityChanged?.Invoke(true);
            }
            else if (oldValue && !newValue)
            {
                // Turned off
                OnHostHandValidityChanged?.Invoke(false);
            }
        }
    }
}
