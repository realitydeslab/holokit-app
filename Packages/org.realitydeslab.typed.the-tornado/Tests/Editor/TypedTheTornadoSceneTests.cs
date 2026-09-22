using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class TypedTheTornadoSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.typed.the-tornado/Assets/Scenes/TypedTheTornado.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arRaycastManager", "_tornado");
        }
    }
}
