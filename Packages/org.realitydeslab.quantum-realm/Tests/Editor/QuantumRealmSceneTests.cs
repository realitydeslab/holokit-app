using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class QuantumRealmSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.quantum-realm/Assets/Scenes/QuantumRealm.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("_arOcclusionManager", "_arPlaneManager", "_arRaycastManager", "_arPlacementIndicator", "_hoverableStartButton", "_buddhaGroupPrefab");
        }

        [Test]
        public void HandTrackingHasDepthManager() => AssertComponentReferences("NetworkHoloKitHandTracker", "_arOcclusionManager");

        [Test]
        public void SpatialAudioHasBothListeners() => AssertComponentReferences("PHASEListenerController", "_phaseListener", "_unityListener");
    }
}
