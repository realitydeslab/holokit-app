using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollectionList")]
    public class MetaObjectCollectionList : ScriptableObject
    {
        public List<MetaObjectCollection> List;

        public List<MetaObjectCollection> FilterByTag(MetaObjectTag tag)
        {
            return List.Where(collection => collection.MetaObjectTags.Contains(tag)).ToList();
        }

        public MetaObjectCollection GetMetaObjectCollection(string bundleId)
        {
            foreach (var metaObjectCollection in List)
            {
                if (metaObjectCollection.BundleId.Equals(bundleId))
                {
                    return metaObjectCollection;
                }
            }
            return null;
        }
    }
}