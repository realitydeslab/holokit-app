using UnityEngine;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObject")]
    public class MetaObject : ScriptableObject
    {
        public Sprite image;

        public VideoClip video;

        public GameObject prefab;

        public string tokenId;

        public MetaObjectCollection collection;
    }
}