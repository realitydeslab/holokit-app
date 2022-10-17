using System.Collections.Generic;
using System.Linq;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseAvatar : HoloKitAppUITemplate_RealityPreferencesPage_ChooseArtifact
    {
        protected override List<NonFungibleCollection> GetCompatibleNonFungibleCollectionList()
        {
            return HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList(HoloKitApp.Instance.CurrentReality).Cast<NonFungibleCollection>().ToList();
        }

        protected override int GetPreferencedArtifactIndex()
        {
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            return CurrentArtifactCollection.GetArtifactIndex(realityPreference.MetaAvatarTokenId);
        }

        protected override void UpdateRealityPreferences(string artifactCollectionId, string artifactTokenId)
        {
            // Save the new avatar collection to global settings
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId] = new RealityPreference()
            {
                MetaAvatarCollectionBundleId = artifactCollectionId,
                MetaAvatarTokenId = artifactTokenId,
                MetaObjectCollectionBundleId = realityPreference.MetaObjectCollectionBundleId,
                MetaObjectTokenId = realityPreference.MetaObjectTokenId
            };
        }
    }
}
