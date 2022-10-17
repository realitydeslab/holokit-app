using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityList")]
    public class RealityList : ScriptableObject
    {
        public List<Reality> List;

        public List<Reality> FilterByTag(RealityTag tag)
        {
            return List.Where(collection => collection.RealityTags.Contains(tag)).ToList();
        }
    }
}