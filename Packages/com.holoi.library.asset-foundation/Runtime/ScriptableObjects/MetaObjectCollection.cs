using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollection")]
    public class MetaObjectCollection : ScriptableObject
    {
        public List<MetaObject> MetaObject;

        public MetaObject coverMetaObject;
        
        public string displayName;

        public string description;

        public string author;

        public SmartContract contract;

        public string imageUrl;
    }
}