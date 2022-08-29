using UnityEngine;
using Holoi.AssetFoundation;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatar")]
    public class MetaAvatar : ScriptableObject
    {
        public Avatar avatar; 

        public Sprite image;

        public VideoClip video;

        public GameObject prefab;

        public bool Rigged;

        public string tokenId;

        public MetaAvatarCollection collection;
    }
}