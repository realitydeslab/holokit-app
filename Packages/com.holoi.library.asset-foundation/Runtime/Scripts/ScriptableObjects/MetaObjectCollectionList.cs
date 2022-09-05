using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollectionList")]
    public class MetaObjectCollectionList : ScriptableObject
    {
        public List<MetaObjectCollection> list;

        public List<MetaObjectCollection> FilterByTag(MetaObjectTag tag)
        {
            return list.Where(collection => collection.tags.Contains(tag)).ToList();
        }
    }
}