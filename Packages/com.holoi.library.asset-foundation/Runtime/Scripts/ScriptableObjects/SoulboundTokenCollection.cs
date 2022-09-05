using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoulboundTokenCollection")]
    public class SoulboundTokenCollection : ScriptableObject
    {
        public List<SoulboundToken> SoulboundToken;

        public string displayName;

        public string description;

        public string author;

        public SmartContract contract;

        public string imageUrl;
    }
}