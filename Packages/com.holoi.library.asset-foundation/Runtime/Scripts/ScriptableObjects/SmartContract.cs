// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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