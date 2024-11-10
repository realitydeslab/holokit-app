// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Linq;
using Holoi.AssetFoundation;
using UnityEngine.Localization.Settings;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseObject : HoloKitAppUITemplate_RealityPreferencesPage_ChooseNonFungible
    {
        protected override List<NonFungibleCollection> GetCompatibleNonFungibleCollectionList()
        {
            return HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaObjectCollectionList(HoloKitApp.Instance.CurrentReality).Cast<NonFungibleCollection>().ToList();
        }

        protected override NonFungibleCollection GetPreferencedNonFungibleCollection()
        {
            return HoloKitApp.Instance.GlobalSettings.GetPreferencedObjectCollection(HoloKitApp.Instance.CurrentReality);
        }

        protected override int GetPreferencedNonFungibleIndex()
        {
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            return CurrentNonFungibleCollection.GetNonFungibleIndex(realityPreference.MetaObjectTokenId);
        }

        protected override void UpdateRealityPreferences(string artifactCollectionId, string artifactTokenId)
        {
            // Save the new object collection to global settings
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId] = new RealityPreference()
            {
                MetaAvatarCollectionBundleId = realityPreference.MetaAvatarCollectionBundleId,
                MetaAvatarTokenId = realityPreference.MetaAvatarTokenId,
                MetaObjectCollectionBundleId = artifactCollectionId,
                MetaObjectTokenId = artifactTokenId
            };
        }

        protected override void UpdateNonFungibleDescription()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (!currentReality.MetaObjectDescription.Equals(""))
            {
                switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                {
                    case "en":
                        NonFungibleDescription.text = currentReality.MetaObjectDescription;
                        break;
                    case "zh-Hans":
                        NonFungibleDescription.text = currentReality.MetaObjectDescription_Chinese;
                        break;
                }
            }
        }
    }
}