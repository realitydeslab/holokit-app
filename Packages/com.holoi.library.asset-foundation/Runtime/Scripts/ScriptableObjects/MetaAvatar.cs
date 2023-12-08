// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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