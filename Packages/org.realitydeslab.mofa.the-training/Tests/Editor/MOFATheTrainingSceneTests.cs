using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class MOFATheTrainingSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.mofa.the-training/Assets/Scenes/MOFATheTraining.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("SpellList", "LifeShieldList", "SpellPool", "_mofaAIPlayerPrefab", "_arPlaneManager", "_arRaycastManager", "_arPlacementManager", "_arPlacementIndicator");
        }
    }
}
