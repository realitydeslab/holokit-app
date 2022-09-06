using UnityEngine;
using UnityEngine.Video;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Reality")]
    public class Reality: ScriptableObject
    {
        public string displayName;

        public string description;

        public string realityId;

        public string author;

        public string version;

        public GameObject thumbnailPrefab;

        public List<VideoClip> previewVideos;

        public List<RealityTag> realityTags;

        public List<MetaAvatarTag> compatibleMetaAvatarTags;

        public List<MetaObjectTag> compatibleMetaObjectTags;
    }
}