// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    public abstract class NonFungibleCollection : ScriptableObject
    {
        public string BundleId;

        public abstract List<NonFungible> NonFungibles { get; }

        public abstract NonFungible CoverNonFungible { get; }

        public string DisplayName;

        public string Description;

        public string Author;

        public SmartContract Contract;

        public string ImageUrl;

        public abstract List<Tag> Tags { get; }

        public int GetNonFungibleIndex(string artifactTokenId)
        {
            int index = -1;
            foreach (var nonFungible in NonFungibles)
            {
                index++;
                if (nonFungible.TokenId.Equals(artifactTokenId))
                {
                    return index;
                }
            }
            return -1;
        }
    }
}