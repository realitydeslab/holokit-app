using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class TypedTheSculptureSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.typed.the-sculpture/Assets/Scenes/TypedTheSculpture.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arOcclusionManager", "_arPlaneManager", "_arRaycastManager", "_arPlacementIndicator", "_hoverableStartButton", "_hostHandVisual", "_sculpturePrefab");
        }
    }
}
