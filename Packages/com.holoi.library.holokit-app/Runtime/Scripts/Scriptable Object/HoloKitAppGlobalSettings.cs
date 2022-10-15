using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Holoi.AssetFoundation;

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

    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppGlobalSettings")]
    public class HoloKitAppGlobalSettings : ScriptableObject
    {
        public RealityList RealityList;

        public RealityList TestRealityList;

        public MetaAvatarCollectionList AvatarCollectionList;

        public MetaObjectCollectionList ObjectCollectionList;

        public bool InstructionEnabled;

        public bool VibrationEnabled;

        // 4K HDR
        public bool HighResHDREnabled;

        public bool UseWifiForMultiplayerEnabled;

        public bool ShowTechInfoEnabled;

        public Dictionary<string, RealityPreference> RealityPreferences;

        // Call this function when application quits.
        public void Save()
        {
            HoloKitAppGlobalSettingsData data = new(this);
            HoloKitAppSaveSystem.SaveGlobalSettings(data);
        }

        // Call this function when application starts.
        public void Load()
        {
            HoloKitAppGlobalSettingsData data = HoloKitAppSaveSystem.LoadGlobalSettings();
            if (data != null)
            {
                InstructionEnabled = data.InstructionEnabled;
                VibrationEnabled = data.VibrationEnabled;
                HighResHDREnabled = data.HighResHDREnabled;
                UseWifiForMultiplayerEnabled = data.UseWifiForMultiplayerEnabled;
                ShowTechInfoEnabled = data.ShowTechInfoEnabled;
                RealityPreferences = data.RealityPreferences;
            }
            else
            {
                SetDefault();
            }
        }

        public void SetDefault()
        {
            InstructionEnabled = true;
            VibrationEnabled = true;
            HighResHDREnabled = true;
            UseWifiForMultiplayerEnabled = false;
            ShowTechInfoEnabled = true;

            // Reality preferences
            RealityPreferences = new();
            foreach (var reality in RealityList.realities)
            {
                // Set the default avatar
                string metaAvatarCollectionId = null;
                string metaAvatarTokenId = null;
                foreach (var avatarCollection in AvatarCollectionList.list)
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
                foreach (var objectCollection in ObjectCollectionList.list)
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
                RealityPreferences[reality.id] = realityPreference;
            }
        }

        public MetaAvatar GetRealityPreferencedAvatar(Reality reality)
        {
            string avatarCollectionId = RealityPreferences[reality.id].MetaAvatarCollectionId;
            string avatarTokenId = RealityPreferences[reality.id].MetaAvatarTokenId;
            foreach (var avatarCollection in AvatarCollectionList.list)
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
            string objectCollectionId = RealityPreferences[reality.id].MetaObjectCollectionId;
            string objectTokenId = RealityPreferences[reality.id].MetaObjectTokenId;
            foreach (var objectCollection in ObjectCollectionList.list)
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
            foreach (var avatarCollection in AvatarCollectionList.list)
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
            foreach (var objectCollection in ObjectCollectionList.list)
            {
                if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                {
                    list.Add(objectCollection);
                }
            }
            return list;
        }

        public MetaAvatarCollection GetMetaAvatarCollection(string id)
        {
            foreach (var metaAvatarCollection in AvatarCollectionList.list)
            {
                if (metaAvatarCollection.id.Equals(id))
                {
                    return metaAvatarCollection;
                }
            }
            return null;
        }

        public MetaObjectCollection GetMetaObjectCollection(string id)
        {
            foreach (var metaObjectCollection in ObjectCollectionList.list)
            {
                if (metaObjectCollection.id.Equals(id))
                {
                    return metaObjectCollection;
                }
            }
            return null;
        }

        public List<Reality> GetAllRealities()
        {
            List<Reality> wholeList = new(RealityList.realities);
            wholeList.AddRange(TestRealityList.realities);
            wholeList = wholeList.Distinct().ToList();
            return wholeList;
        }

        public int GetRealityIndex(Reality reality)
        {
            int realityIndex = 0;
            foreach (var realityInstance in RealityList.realities)
            {
                if (reality.Equals(realityInstance))
                {
                    return realityIndex;
                }
                realityIndex++;
            }
            return 0;
        }
    }
}
