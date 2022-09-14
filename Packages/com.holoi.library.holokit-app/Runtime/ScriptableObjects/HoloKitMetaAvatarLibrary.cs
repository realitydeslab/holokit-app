using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections.Generic;

namespace Holoi.Library.HoloKitApp
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitMetaAvatarLibrary")]
    public class HoloKitMetaAvatarLibrary : ScriptableObject
    {
        public List<MetaAvatarCollection> metaAvatarCollections;
    }
}