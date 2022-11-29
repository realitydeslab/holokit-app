using System.Collections.Generic;
using System.Linq;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseAvatar : HoloKitAppUITemplate_RealityPreferencesPage_ChooseNonFungible
    {
        protected override List<NonFungibleCollection> GetCompatibleNonFungibleCollectionList()
        {
            return HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList().Cast<NonFungibleCollection>().ToList();
        }

        protected override NonFungibleCollection GetPreferencedNonFungibleCollection()
        {
            return HoloKitApp.Instance.GlobalSettings.GetPreferencedAvatarCollection();
        }

        protected override int GetPreferencedNonFungibleIndex()
        {
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            return CurrentNonFungibleCollection.GetNonFungibleIndex(realityPreference.MetaAvatarTokenId);
        }

        protected override void UpdateRealityPreferences(string nonFungibleCollectionId, string nonFungibleTokenId)
        {
            // Save the new avatar collection to global settings
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId] = new RealityPreference()
            {
                MetaAvatarCollectionBundleId = nonFungibleCollectionId,
                MetaAvatarTokenId = nonFungibleTokenId,
                MetaObjectCollectionBundleId = realityPreference.MetaObjectCollectionBundleId,
                MetaObjectTokenId = realityPreference.MetaObjectTokenId
            };
        }

        protected override void UpdateNonFungibleDescription()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (!currentReality.MetaAvatarDescription.Equals(""))
            {
                NonFungibleDescription.text = currentReality.MetaAvatarDescription;
            }
        }
    }
}
