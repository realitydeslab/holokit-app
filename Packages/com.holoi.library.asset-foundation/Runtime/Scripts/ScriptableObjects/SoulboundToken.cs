// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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