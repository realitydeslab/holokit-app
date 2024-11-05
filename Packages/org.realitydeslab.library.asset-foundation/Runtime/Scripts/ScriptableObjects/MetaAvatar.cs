// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatar")]
    public class MetaAvatar : NonFungible
    {
        public bool Rigged;

        public Avatar UnityAvatar;

        public override NonFungibleCollection NonFungibleCollection => MetaAvatarCollection;

        public MetaAvatarCollection MetaAvatarCollection;
    }
}