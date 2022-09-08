using UnityEngine;
using Holoi.AssetFoundation;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoulboundToken")]
    public class SoulboundToken : ScriptableObject
    {
        public Sprite image;

        public VideoClip video;

        public GameObject prefab;

        public string tokenId;

        public SoulboundTokenCollection collection;
    }
}