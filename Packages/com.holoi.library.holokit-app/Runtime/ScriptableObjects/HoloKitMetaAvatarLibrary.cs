using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitMetaAvatarLibrary")]
    public class HoloKitMetaAvatarLibrary : ScriptableObject
    {
        public List<MetaAvatarCollection> metaAvatarCollections;
    }
}