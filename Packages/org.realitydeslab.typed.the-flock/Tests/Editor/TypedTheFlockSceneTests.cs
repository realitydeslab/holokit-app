using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class TypedTheFlockSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.typed.the-flock/Assets/Scenes/TypedTheFlock.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arOcclusionManager", "_boidVfx", "_boidAlgorithm");
        }

        [Test]
        public void SpatialAudioHasBothListeners() => AssertComponentReferences("PHASEListenerController", "_phaseListener", "_unityListener");
    }
}
