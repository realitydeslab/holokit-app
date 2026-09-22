using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class MOFATheHuntingSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.mofa.the-hunting/Assets/Scenes/MOFATheHunting.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arPlaneManager", "_arRaycastManager", "_arPlacementManager", "_arPlacementIndicator", "_invisibleFloorPrefab", "_portalPrefab", "_dragonPrefab");
        }
    }
}
