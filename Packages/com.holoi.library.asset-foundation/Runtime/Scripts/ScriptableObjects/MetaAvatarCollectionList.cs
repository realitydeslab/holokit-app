using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatarCollectionList")]
    public class MetaAvatarCollectionList : ScriptableObject
    {
        public List<MetaAvatarCollection> List;

        public List<MetaAvatarCollection> FilterByTag(MetaAvatarTag tag)
        {
            return List.Where(collection => collection.MetaAvatarTags.Contains(tag)).ToList();
        }
    }
}