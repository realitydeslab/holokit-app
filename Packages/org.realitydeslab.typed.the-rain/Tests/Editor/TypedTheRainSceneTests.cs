using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class TypedTheRainSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.typed.the-rain/Assets/Scenes/TypedTheRain.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arPlaneManager", "_arRaycastManager", "_hostCenterEyeTransform", "_rainVfx");
        }
    }
}
