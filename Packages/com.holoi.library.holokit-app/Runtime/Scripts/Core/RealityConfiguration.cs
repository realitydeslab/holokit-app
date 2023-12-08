// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
