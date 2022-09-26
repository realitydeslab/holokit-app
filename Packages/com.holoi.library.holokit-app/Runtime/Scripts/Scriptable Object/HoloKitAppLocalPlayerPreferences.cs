using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;
using System;

namespace Holoi.Library.HoloKitApp
{
    [Serializable]
    public struct RealityPreference
    {
        public string MetaAvatarCollectionId;

        public string MetaAvatarTokenId;

        public string MetaObjectCollectionId;

        public string MetaObjectTokenId;
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppLocalPlayerPreferences")]
    public class HoloKitAppLocalPlayerPreferences : ScriptableObject
    {
        [SerializeField] private RealityList _realityList;

        [SerializeField] private MetaAvatarCollectionList _avatarCollectionList;

        [SerializeField] private MetaObjectCollectionList _objectCollectionList;

        public Dictionary<string, RealityPreference> RealityPreferences;

        // Call this function when application quits.
        public void Save()
        {
            HoloKitAppLocalPlayerPreferencesData data = new(this);
            HoloKitAppSaveSystem.SaveLocalPlayerPreferences(data);
        }

        // Call this function when application starts.
        public void Load()
        {
            HoloKitAppLocalPlayerPreferencesData data = HoloKitAppSaveSystem.LoadLocalPlayerPreferences();
            if (data != null)
            {
                RealityPreferences = data.RealityPreferences;
            }
            else
            {
                RealityPreferences = new();
                // Set default preferences
                foreach (var reality in _realityList.realities)
                {
                    // Set the default avatar
                    string metaAvatarCollectionId = null;
                    string metaAvatarTokenId = null;
                    foreach (var avatarCollection in _avatarCollectionList.list)
                    {
                        if (reality.IsCompatibleWithMetaAvatarCollection(avatarCollection))
                        {
                            metaAvatarCollectionId = avatarCollection.id;
                            metaAvatarTokenId = avatarCollection.coverMetaAvatar.tokenId;
                            break;
                        }
                    }

                    // Set the default object
                    string metaObjectCollectionId = null;
                    string metaObjectTokenId = null;
                    foreach (var objectCollection in _objectCollectionList.list)
                    {
                        if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                        {
                            metaObjectCollectionId = objectCollection.id;
                            metaObjectTokenId = objectCollection.coverMetaObject.tokenId;
                            break;
                        }
                    }

                    RealityPreference realityPreference = new()
                    {
                        MetaAvatarCollectionId = metaAvatarCollectionId,
                        MetaAvatarTokenId = metaAvatarTokenId,
                        MetaObjectCollectionId = metaObjectCollectionId,
                        MetaObjectTokenId = metaObjectTokenId
                    };
                    RealityPreferences[reality.realityId] = realityPreference;
                }
            }
        }

        public MetaAvatar GetRealityPreferencedAvatar(Reality reality)
        {
            string avatarCollectionId = RealityPreferences[reality.realityId].MetaAvatarCollectionId;
            string avatarTokenId = RealityPreferences[reality.realityId].MetaAvatarTokenId;
            foreach (var avatarCollection in _avatarCollectionList.list)
            {
                if (avatarCollection.id.Equals(avatarCollectionId))
                {
                    foreach (var avatar in avatarCollection.metaAvatars)
                    {
                        if (avatar.tokenId.Equals(avatarTokenId))
                        {
                            return avatar;
                        }
                    }
                }
            }
            return null;
        }

        public MetaObject GetRealityPreferencedObject(Reality reality)
        {
            string objectCollectionId = RealityPreferences[reality.realityId].MetaObjectCollectionId;
            string objectTokenId = RealityPreferences[reality.realityId].MetaObjectTokenId;
            foreach (var objectCollection in _objectCollectionList.list)
            {
                if (objectCollection.id.Equals(objectCollectionId))
                {
                    foreach (var metaObject in objectCollection.metaObjects)
                    {
                        if (metaObject.tokenId.Equals(objectTokenId))
                        {
                            return metaObject;
                        }
                    }
                }
            }
            return null;
        }

        public List<MetaAvatarCollection> GetCompatibleMetaAvatarCollectionList(Reality reality)
        {
            List<MetaAvatarCollection> list = new();
            foreach (var avatarCollection in _avatarCollectionList.list)
            {
                if (reality.IsCompatibleWithMetaAvatarCollection(avatarCollection))
                {
                    list.Add(avatarCollection);
                }
            }
            return list;
        }

        public List<MetaObjectCollection> GetCompatibleMetaObjectCollectionList(Reality reality)
        {
            List<MetaObjectCollection> list = new();
            foreach (var objectCollection in _objectCollectionList.list)
            {
                if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                {
                    list.Add(objectCollection);
                }
            }
            return list;
        }
    }
}
