using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class TypedTheHairSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.typed.the-hair/Assets/Scenes/TypedTheHair.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arOcclusionManager");
        }
    }
}
