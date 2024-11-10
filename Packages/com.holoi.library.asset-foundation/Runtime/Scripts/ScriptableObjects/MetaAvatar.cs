// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
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