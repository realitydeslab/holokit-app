using UnityEngine;
using UnityEngine.Video;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityCollection")]
    public class RealityCollection : ScriptableObject
    {
        public List<Reality> realityCollection;
    }
}