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
        public string MetaAvatarCollectionBundleId;

        public string MetaAvatarTokenId;

        public string MetaObjectCollectionBundleId;

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

        // This is temporary
        private const string AmbersAvatarCollectionBundleId = "com.holoi.avatar.meebits";

        private const string AmbersAvatarTokenId = "5958";

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
            foreach (var reality in RealityList.List)
            {
                // Set the default avatar
                string metaAvatarCollectionId = null;
                string metaAvatarTokenId = null;
                foreach (var avatarCollection in AvatarCollectionList.List)
                {
                    if (reality.IsCompatibleWithMetaAvatarCollection(avatarCollection))
                    {
                        metaAvatarCollectionId = avatarCollection.BundleId;
                        metaAvatarTokenId = avatarCollection.CoverMetaAvatar ? avatarCollection.CoverMetaAvatar.TokenId : null;
                        break;
                    }
                }

                // Set the default object
                string metaObjectCollectionId = null;
                string metaObjectTokenId = null;
                foreach (var objectCollection in ObjectCollectionList.List)
                {
                    if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                    {
                        metaObjectCollectionId = objectCollection.BundleId;
                        metaObjectTokenId = objectCollection.CoverMetaObject ? objectCollection.CoverMetaObject.TokenId : null;
                        break;
                    }
                }

                RealityPreference realityPreference = new()
                {
                    MetaAvatarCollectionBundleId = metaAvatarCollectionId,
                    MetaAvatarTokenId = metaAvatarTokenId,
                    MetaObjectCollectionBundleId = metaObjectCollectionId,
                    MetaObjectTokenId = metaObjectTokenId
                };
                RealityPreferences[reality.BundleId] = realityPreference;
            }
        }

        public MetaAvatarCollection GetPreferencedAvatarCollection(Reality reality)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
            string avatarCollectionBundleId = RealityPreferences[reality.BundleId].MetaAvatarCollectionBundleId;
            foreach (var avatarCollection in AvatarCollectionList.List)
            {
                if (avatarCollection.BundleId.Equals(avatarCollectionBundleId))
                {
                    return avatarCollection;
                }
            }
            return null;
        }

        public MetaObjectCollection GetPreferencedObjectCollection(Reality reality)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
            string objectCollectionBundleId = RealityPreferences[reality.BundleId].MetaObjectCollectionBundleId;
            foreach (var objectCollection in ObjectCollectionList.List)
            {
                if (objectCollection.BundleId.Equals(objectCollectionBundleId))
                {
                    return objectCollection;
                }
            }
            return null;
        }

        public MetaAvatar GetPreferencedAvatar(Reality reality)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
            string avatarTokenId = RealityPreferences[reality.BundleId].MetaAvatarTokenId;
            var avatarCollection = GetPreferencedAvatarCollection(reality);
            foreach (var avatar in avatarCollection.MetaAvatars)
            {
                if (avatar.TokenId.Equals(avatarTokenId))
                {
                    return avatar;
                }
            }
            return null;
        }

        public MetaObject GetPreferencedObject(Reality reality)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
            string objectTokenId = RealityPreferences[reality.BundleId].MetaObjectTokenId;
            var objectCollection = GetPreferencedObjectCollection(reality);
            foreach (var metaObject in objectCollection.MetaObjects)
            {
                if (metaObject.TokenId.Equals(objectTokenId))
                {
                    return metaObject;
                }
            }
            return null;
        }

        public List<MetaAvatarCollection> GetCompatibleMetaAvatarCollectionList(Reality reality)
        {
            List<MetaAvatarCollection> list = new();
            foreach (var avatarCollection in AvatarCollectionList.List)
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
            foreach (var objectCollection in ObjectCollectionList.List)
            {
                if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                {
                    list.Add(objectCollection);
                }
            }
            return list;
        }

        public MetaAvatarCollection GetMetaAvatarCollection(string bundleId)
        {
            foreach (var metaAvatarCollection in AvatarCollectionList.List)
            {
                if (metaAvatarCollection.BundleId.Equals(bundleId))
                {
                    return metaAvatarCollection;
                }
            }
            return null;
        }

        public MetaObjectCollection GetMetaObjectCollection(string bundleId)
        {
            foreach (var metaObjectCollection in ObjectCollectionList.List)
            {
                if (metaObjectCollection.BundleId.Equals(bundleId))
                {
                    return metaObjectCollection;
                }
            }
            return null;
        }

        public List<Reality> GetAllRealities()
        {
            List<Reality> wholeList = new(RealityList.List);
            wholeList.AddRange(TestRealityList.List);
            wholeList = wholeList.Distinct().ToList();
            return wholeList;
        }

        public int GetRealityIndex(Reality reality)
        {
            int realityIndex = 0;
            foreach (var realityInstance in RealityList.List)
            {
                if (reality.Equals(realityInstance))
                {
                    return realityIndex;
                }
                realityIndex++;
            }
            return 0;
        }

        public MetaAvatarCollection GetPreferencedMetaAvatarCollection(Reality reality)
        {
            RealityPreference realityPreference = RealityPreferences[reality.BundleId];
            foreach (var metaAvatarCollection in AvatarCollectionList.List)
            {
                if (metaAvatarCollection.BundleId.Equals(realityPreference.MetaAvatarCollectionBundleId))
                {
                    return metaAvatarCollection;
                }
            }
            return null;
        }

        public MetaAvatarCollection GetAmbersAvatarCollection()
        {
            foreach (var avatarCollection in AvatarCollectionList.List)
            {
                if (avatarCollection.BundleId.Equals(AmbersAvatarCollectionBundleId))
                {
                    return avatarCollection;
                }
            }
            return null;
        }

        public MetaAvatar GetAmbersAvatar()
        {
            MetaAvatarCollection avatarCollection = GetAmbersAvatarCollection();
            foreach (var avatar in avatarCollection.MetaAvatars)
            {
                if (avatar.TokenId.Equals(AmbersAvatarTokenId))
                {
                    return avatar;
                }
            }
            return null;
        }
    }
}
