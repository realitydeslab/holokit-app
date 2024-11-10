// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
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