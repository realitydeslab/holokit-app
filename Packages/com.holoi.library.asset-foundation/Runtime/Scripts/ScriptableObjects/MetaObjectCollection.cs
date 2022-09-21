using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollection")]
    public class MetaObjectCollection : ScriptableObject
    {
        public string id;

        public List<MetaObject> metaObjects;

        public MetaObject coverMetaObject;
        
        public string displayName;

        public string description;

        public string author;

        public SmartContract contract;

        public string imageUrl;

        public List<MetaObjectTag> tags;
    }
}