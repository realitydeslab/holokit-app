using System.Collections.Generic;
using System.Linq;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseAvatar : HoloKitAppUITemplate_RealityPreferencesPage_ChooseArtifact
    {
        protected override List<ArtifactCollection> GetCompatibleArtifactCollectionList()
        {
            return HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList(HoloKitApp.Instance.CurrentReality).Cast<ArtifactCollection>().ToList();
        }
    }
}
