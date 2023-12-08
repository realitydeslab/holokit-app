// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    public abstract class NonFungible : ScriptableObject
    {
        public string TokenId;

        public Sprite Image;

        public VideoClip Video;

        public GameObject Prefab;

        public abstract NonFungibleCollection NonFungibleCollection { get; }
    }
}