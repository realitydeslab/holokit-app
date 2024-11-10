// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityTag")]
    public class RealityTag : Tag 
    {
        public string BundleId;

        public string DisplayName;

        public string DisplayName_Chinese;
    }
}