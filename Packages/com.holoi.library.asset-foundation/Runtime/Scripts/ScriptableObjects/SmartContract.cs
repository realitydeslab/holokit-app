// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SmartContract")]
    public class SmartContract : ScriptableObject
    {
        public string Address;

        public string ChainName = "ethereum";

        public string OpenseaUrl(string tokenId) {
            return $"https://opensea.io/assets/{ChainName}/{Address}/{tokenId}";
        }
    }
}