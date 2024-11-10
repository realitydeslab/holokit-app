// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Holoi.Library.HoloKitApp
{
    public class RealityConfiguration : MonoBehaviour
    {
        public GameObject PlayerPrefab;

        public List<GameObject> NetworkPrefabs;

        public List<UI.HoloKitAppUIPanel> UIPanelPrefabs;

        public List<UI.HoloKitAppUIRealitySettingTab> UIRealitySettingTabPrefabs;

        public UniversalRenderPipelineAsset UrpAsset;

        [Tooltip("Whether the players' poses are synced across the network by default")]
        public bool SyncPlayerPoseByDefault = false;
    }
}
