using UnityEngine;
using UnityEngine.Video;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;
using System;

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

        public GameObject realityManager;

        public string waterMarkField = "";

        public bool IsCompatibleWithMetaAvatarCollection(MetaAvatarCollection avatarCollection)
        {
            int count = 0;
            foreach (var tag in compatibleMetaAvatarTags)
            {
                foreach (var tag2 in avatarCollection.tags)
                {
                    if (tag.Equals(tag2))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == compatibleMetaAvatarTags.Count)
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
            int count = 0;
            foreach (var tag in compatibleMetaObjectTags)
            {
                foreach (var tag2 in objectCollection.tags)
                {
                    if (tag.Equals(tag2))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == compatibleMetaObjectTags.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}