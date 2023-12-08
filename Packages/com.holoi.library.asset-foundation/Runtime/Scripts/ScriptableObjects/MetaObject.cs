// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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