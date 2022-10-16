using UnityEngine;
using UnityEngine.Video;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityList")]
    public class RealityList : ScriptableObject
    {
        public List<Reality> realities;

        public List<Reality> FilterByTag(RealityTag tag)
        {
            return realities.Where(collection => collection.RealityTags.Contains(tag)).ToList();
        }
    }
}