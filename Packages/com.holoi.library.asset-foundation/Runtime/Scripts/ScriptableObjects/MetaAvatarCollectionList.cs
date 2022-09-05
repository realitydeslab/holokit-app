using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatarCollectionList")]
    public class MetaAvatarCollectionList : ScriptableObject
    {
        public List<MetaAvatarCollection> list;

        public List<MetaAvatarCollection> FilterByTag(MetaAvatarTag tag)
        {
            return list.Where(collection => collection.tags.Contains(tag)).ToList();
        }
    }
}