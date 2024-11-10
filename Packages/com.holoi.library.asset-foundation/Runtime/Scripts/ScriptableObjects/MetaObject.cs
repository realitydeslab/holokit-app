// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObject")]
    public class MetaObject: NonFungible
    {
        public override NonFungibleCollection NonFungibleCollection => MetaObjectCollection;

        public MetaObjectCollection MetaObjectCollection;
    }
}