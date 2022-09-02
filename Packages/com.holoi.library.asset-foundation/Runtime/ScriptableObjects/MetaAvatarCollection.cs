using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatarCollection")]
    public class MetaAvatarCollection : ScriptableObject
    {
        public List<MetaAvatar> MetaAvatar;

        public MetaAvatar coverMetaAvatar;
        
        public string displayName;

        public string description;

        public string author;

        public SmartContract contract;

        public string imageUrl;
    }
}