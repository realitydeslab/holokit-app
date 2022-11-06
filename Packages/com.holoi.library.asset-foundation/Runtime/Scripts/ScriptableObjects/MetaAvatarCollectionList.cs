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

        public MetaAvatarCollection GetMetaAvatarCollection(string bundleId)
        {
            foreach (var metaAvatarCollection in List)
            {
                if (metaAvatarCollection.BundleId.Equals(bundleId))
                {
                    return metaAvatarCollection;
                }
            }
            return null;
        }
    }
}