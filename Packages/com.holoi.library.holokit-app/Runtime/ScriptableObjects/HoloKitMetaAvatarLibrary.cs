using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections.Generic;

namespace Holoi.HoloKit.App
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitMetaAvatarLibrary")]
    public class HoloKitMetaAvatarLibrary : ScriptableObject
    {
        public List<MetaAvatarCollection> metaAvatarCollections;
    }
}