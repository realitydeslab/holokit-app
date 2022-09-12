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
        public string DisplayName;

        public string Description;

        public string RealityId;

        public string Author;

        public string Version;

        public GameObject ThumbnailPrefab;

        public List<VideoClip> PreviewVideos;

        public List<RealityTag> RealityTags;

        public List<MetaAvatarTag> CompatibleMetaAvatarTags;

        public List<MetaObjectTag> CompatibleMetaObjectTags;

        public GameObject RealityManager;
    }
}