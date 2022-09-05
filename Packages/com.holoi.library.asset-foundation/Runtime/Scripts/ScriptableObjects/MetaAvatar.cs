using UnityEngine;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatar")]
    public class MetaAvatar : ScriptableObject
    {
        public Avatar unityAvatar; 

        public Sprite image;

        public VideoClip video;

        public GameObject prefab;

        public bool rigged;

        public string tokenId;

        public MetaAvatarCollection collection;
    }
}