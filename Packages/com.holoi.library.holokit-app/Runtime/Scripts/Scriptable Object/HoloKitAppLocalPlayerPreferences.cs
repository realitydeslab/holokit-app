using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;
using System;

namespace Holoi.Library.HoloKitApp
{
    public struct RealityPreference
    {
        public MetaAvatar MetaAvatar;

        public MetaObject MetaObject;
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppLocalPlayerPreferences")]
    public class HoloKitAppLocalPlayerPreferences : ScriptableObject
    {
        [SerializeField] private RealityList _realityList;

        [SerializeField] private MetaAvatarCollectionList _avatarCollectionList;

        [SerializeField] private MetaObjectCollectionList _objectCollectionList;

        public Dictionary<Reality, RealityPreference> RealityPreferences;

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
                    MetaAvatar metaAvatar = null;
                    foreach (var avatarCollection in _avatarCollectionList.list)
                    {
                        if (reality.IsCompatibleWithMetaAvatarCollection(avatarCollection))
                        {
                            metaAvatar = avatarCollection.coverMetaAvatar;
                            break;
                        }
                    }

                    // Set the default object
                    MetaObject metaObject = null;
                    foreach (var objectCollection in _objectCollectionList.list)
                    {
                        if (reality.IsCompatibleWithMetaObjectCollection(objectCollection))
                        {
                            metaObject = objectCollection.coverMetaObject;
                            break;
                        }
                    }

                    RealityPreference realityPreference = new()
                    {
                        MetaAvatar = metaAvatar,
                        MetaObject = metaObject
                    };
                    RealityPreferences[reality] = realityPreference;
                }
            }
        }
    }
}
