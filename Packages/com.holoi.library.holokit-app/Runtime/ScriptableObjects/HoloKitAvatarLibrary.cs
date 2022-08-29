using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAvatarLibrary")]
    public class HoloKitAvatarLibrary : ScriptableObject
    {
        public List<AvatarCollection> avatarCollections;
    }
}