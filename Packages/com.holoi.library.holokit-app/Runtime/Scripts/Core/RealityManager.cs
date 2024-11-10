// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public struct RealitySessionData
    {
        public string RealityBundleId;

        public float SessionDuration;

        public int PlayerCount;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RealityConfiguration))]
    public abstract class RealityManager : NetworkBehaviour
    {
        public RealityConfiguration Config => _config;

        private RealityConfiguration _config;

        /// <summary>
        /// This function should be called at the very beginning of a reality session
        /// by the system.
        /// </summary>
        public void FindConfig()
        {
            _config = GetComponent<RealityConfiguration>();
        }

        public void SetupURPAsset()
        {
            if (_config.UrpAsset != null)
            {
                GraphicsSettings.defaultRenderPipeline = _config.UrpAsset;
            }
        }

        public RealitySessionData GetRealitySessionData()
        {
            RealitySessionData realitySessionData = new()
            {
                RealityBundleId = HoloKitApp.Instance.CurrentReality.BundleId,
                SessionDuration = Time.time - HoloKitCameraManager.Instance.ARSessionStartTime,
                PlayerCount = HoloKitApp.Instance.MultiplayerManager.PlayerCount
            };
            return realitySessionData;
        }
    }
}