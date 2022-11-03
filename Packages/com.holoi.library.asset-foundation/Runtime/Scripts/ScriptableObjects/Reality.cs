using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Reality")]
    public class Reality: ScriptableObject
    {
        // A unique string to distinguish this reality from other realities
        public string BundleId;

        public string DisplayName;

        public string Description;

        public string Author;

        public string Version;

        public GameObject ThumbnailPrefab;

        public List<VideoClip> PreviewVideos;

        public List<RealityTag> RealityTags;

        public List<MetaAvatarTag> CompatibleMetaAvatarTags;

        public List<MetaObjectTag> CompatibleMetaObjectTags;

        public SceneField Scene;

        public string WatermarkField = "";

        public bool IsCompatibleWithMetaAvatarCollection(MetaAvatarCollection avatarCollection)
        {
            if (CompatibleMetaAvatarTags.Count == 0)
            {
                return false;
            }

            int count = 0;
            foreach (var tag in CompatibleMetaAvatarTags)
            {
                foreach (var tag2 in avatarCollection.MetaAvatarTags)
                {
                    if (tag.Equals(tag2))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == CompatibleMetaAvatarTags.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCompatibleWithMetaObjectCollection(MetaObjectCollection objectCollection)
        {
            if (CompatibleMetaObjectTags.Count == 0)
            {
                return false;
            }

            int count = 0;
            foreach (var tag in CompatibleMetaObjectTags)
            {
                foreach (var tag2 in objectCollection.MetaObjectTags)
                {
                    if (tag.Equals(tag2))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == CompatibleMetaObjectTags.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsMultiplayerSupported()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.support-multiplayer"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLiDARRequired()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.lidar"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPhaseEnabled()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.phase"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}