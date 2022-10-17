using System.Collections.Generic;
using System.Linq;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseObject : HoloKitAppUITemplate_RealityPreferencesPage_ChooseArtifact
    {
        protected override List<ArtifactCollection> GetCompatibleArtifactCollectionList()
        {
            return HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaObjectCollectionList(HoloKitApp.Instance.CurrentReality).Cast<ArtifactCollection>().ToList();
        }

        protected override int GetPreferencedArtifactIndex()
        {
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            return CurrentArtifactCollection.GetArtifactIndex(realityPreference.MetaObjectTokenId);
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
    }
}