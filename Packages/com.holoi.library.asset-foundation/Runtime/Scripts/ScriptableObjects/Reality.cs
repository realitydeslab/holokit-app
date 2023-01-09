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

        public string DisplayName_Chinese;

        [TextArea]
        public string Description;

        [TextArea]
        public string Description_Chinese;

        public string Author;

        public string Author_Chinese;

        public string Version;

        public GameObject ThumbnailPrefab;

        public List<VideoClip> PreviewVideos;

        public List<VideoClip> TutorialVideos;

        [TextArea]
        public string HardwareRequirement;

        [TextArea]
        public string HardwareRequirement_Chinese;

        public List<RealityTag> RealityTags;

        [TextArea, Tooltip("A description on why this reality needs to use meta avatars")]
        public string MetaAvatarDescription;

        [TextArea]
        public string MetaAvatarDescription_Chinese;

        public List<MetaAvatarTag> CompatibleMetaAvatarTags;

        [TextArea, Tooltip("A description on why this reality needs to use meta objects")]
        public string MetaObjectDescription;

        [TextArea]
        public string MetaObjectDescription_Chinese;

        public List<MetaObjectTag> CompatibleMetaObjectTags;

        public SceneField Scene;

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
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.multiplayer"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHostMultiplayerSupported()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.host-multiplayer"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSpectatorViewSupported()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.spectator-view"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPuppeteerSupported()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.puppeteer"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHostPuppeteerSupported()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.host-puppeteer"))
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

        public bool IsPhaseRequired()
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

        public bool IsHapticsRequired()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.haptics"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAppleWatchRequired()
        {
            foreach (var realityTag in RealityTags)
            {
                if (realityTag.BundleId.Equals("com.holoi.asset-foundation.apple-watch"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}