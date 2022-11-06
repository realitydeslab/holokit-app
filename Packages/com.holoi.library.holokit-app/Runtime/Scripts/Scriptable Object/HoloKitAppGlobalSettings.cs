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

        public bool PhaseEnabled;

        public bool HighResHDREnabled; // 4K HDR

        public bool RecordMicrophone;

        public bool WatermarkEnabled;

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
                PhaseEnabled = data.PhaseEnabled;
                HighResHDREnabled = data.HighResHDREnabled;
                RecordMicrophone = data.RecordMicrophone;
                WatermarkEnabled = data.WatermarkEnabled;
                UseWifiForMultiplayerEnabled = data.UseWifiForMultiplayerEnabled;
                ShowTechInfoEnabled = data.ShowTechInfoEnabled;
                RealityPreferences = data.RealityPreferences;
                // TODO: Check if the loaded data is valid?
                if (!ValidateCurrentSettings())
                {
                    Debug.Log("[GlobalSettings] The old settings are not valid anymore due to a recent update");
                    LoadDefaultSettings();
                }
                else
                {
                    Debug.Log("[GlobalSettings] The current loaded settings are valid");
                }
            }
            else
            {
                LoadDefaultSettings();
            }
        }

        public void LoadDefaultSettings()
        {
            InstructionEnabled = true;
            VibrationEnabled = true;
            PhaseEnabled = false;
            HighResHDREnabled = true;
            RecordMicrophone = true;
            WatermarkEnabled = true;
            UseWifiForMultiplayerEnabled = false;
            ShowTechInfoEnabled = true;

            // Reality preferences
            RealityPreferences = new();
            foreach (var reality in GetAllRealities())
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
            Debug.Log("[GlobalSettings] Loaded default global settings");
        }

        /// <summary>
        /// If the current settings loaded from disk are not valid, we should overwrite them with default settings.
        /// </summary>
        /// <returns></returns>
        private bool ValidateCurrentSettings()
        {
            foreach (var reality in GetAllRealities())
            {
                if (RealityPreferences.ContainsKey(reality.BundleId))
                {
                    if (GetCompatibleMetaAvatarCollectionList(reality).Count > 0)
                    {
                        MetaAvatarCollection preferencedAvatarCollection = null;
                        foreach (var avatarCollection in AvatarCollectionList.List)
                        {
                            if (avatarCollection.BundleId.Equals(RealityPreferences[reality.BundleId].MetaAvatarCollectionBundleId))
                            {
                                preferencedAvatarCollection = avatarCollection;
                                break;
                            }
                        }
                        if (preferencedAvatarCollection == null)
                        {
                            return false;
                        }
                        bool foundAvatar = false;
                        foreach (var avatar in preferencedAvatarCollection.MetaAvatars)
                        {
                            if (avatar.TokenId.Equals(RealityPreferences[reality.BundleId].MetaAvatarTokenId))
                            {
                                foundAvatar = true;
                                break;
                            }
                        }
                        if (!foundAvatar)
                        {
                            return false;
                        }
                    }
                    
                    if (GetCompatibleMetaObjectCollectionList(reality).Count > 0)
                    {
                        MetaObjectCollection preferencedObjectCollection = null;
                        foreach (var objectCollection in ObjectCollectionList.List)
                        {
                            if (objectCollection.BundleId.Equals(RealityPreferences[reality.BundleId].MetaObjectCollectionBundleId))
                            {
                                preferencedObjectCollection = objectCollection;
                                break;
                            }
                        }
                        if (preferencedObjectCollection == null)
                        {
                            return false;
                        }
                        bool foundObject = false;
                        foreach (var metaObject in preferencedObjectCollection.MetaObjects)
                        {
                            if (metaObject.TokenId.Equals(RealityPreferences[reality.BundleId].MetaObjectTokenId))
                            {
                                foundObject = true;
                                break;
                            }
                        }
                        if (!foundObject)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public MetaAvatarCollection GetPreferencedAvatarCollection(Reality reality = null)
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

        public MetaObjectCollection GetPreferencedObjectCollection(Reality reality = null)
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

        public MetaAvatar GetPreferencedAvatar(Reality reality = null)
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

        public MetaObject GetPreferencedObject(Reality reality = null)
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

        public List<MetaAvatarCollection> GetCompatibleMetaAvatarCollectionList(Reality reality = null)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
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

        public List<MetaObjectCollection> GetCompatibleMetaObjectCollectionList(Reality reality = null)
        {
            if (reality == null)
            {
                reality = HoloKitApp.Instance.CurrentReality;
            }
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

        /// <summary>
        /// Get realities from both formal and test lists.
        /// </summary>
        /// <returns></returns>
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
    }
}
