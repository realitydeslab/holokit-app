using UnityEngine;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    public abstract class NonFungible : ScriptableObject
    {
        public string TokenId;

        public Sprite Image;

        public VideoClip Video;

        public GameObject Prefab;

        public abstract NonFungibleCollection NonFungibleCollection { get; }
    }
}