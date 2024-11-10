// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoulboundToken")]
    public class SoulboundToken : NonFungible 
    {
        public override NonFungibleCollection NonFungibleCollection => SBTCollection;

        public SoulboundTokenCollection SBTCollection;
    }
}