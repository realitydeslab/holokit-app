using HoloKit;
using HoloKit.Tests;
using Holoi.Library.ARUX;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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

        [Test]
        public void AiAvatarConfigurationsResolveTheirCollections()
        {
            var list = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                "Packages/org.realitydeslab.mofa.the-training/Assets/Scriptable Objects/Mofa Avatar Collection Params List.asset");
            Assert.That(list, Is.Not.Null);
            var entries = new SerializedObject(list).FindProperty("List");
            Assert.That(entries.arraySize, Is.GreaterThan(0));
            for (var i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i).objectReferenceValue;
                Assert.That(entry, Is.Not.Null, $"Avatar configuration {i} is missing.");
                var collection = new SerializedObject(entry).FindProperty("MetaAvatarCollection").objectReferenceValue;
                Assert.That(collection, Is.Not.Null, $"Avatar configuration {i} has no avatar collection.");
            }
        }

        // The opponent wears its avatar's own SH_Meebits materials on every platform. An unlit iOS stand-in hid a build that
        // lacked SH_Meebits' shadows-off variant (PlayableProductRenderingAssetTests.RealityUrpAssetsMatchTheDefaultAssetsMainLightShadowSupport) and made it look dark.
        [Test]
        public void AiAvatarWearsItsOwnMaterialsAndWand()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/org.realitydeslab.mofa.the-training/Assets/Prefabs/Mofa AI Player.prefab");
            Assert.That(prefab, Is.Not.Null);
            var player = prefab.GetComponent("MofaAIPlayer");
            Assert.That(player, Is.Not.Null);
            using var serialized = new SerializedObject(player);
            Assert.That(serialized.FindProperty("_iosAvatarFallbackMaterial"), Is.Null, "The opponent needs no stand-in material.");
            var weapon = serialized.FindProperty("_aiWeaponPrefab").objectReferenceValue as GameObject;
            Assert.That(weapon, Is.Not.Null, "AI hand-held wand prefab is missing.");
        }

        [Test]
        public void DefaultPlacementHeightDoesNotChangeWithHeadPitch()
        {
            var head = new GameObject("Test center eye");
            try
            {
                head.transform.position = new Vector3(0f, 1.7f, 0f);
                head.transform.rotation = Quaternion.Euler(30f, 45f, 0f);
                var position = ARPlacementManager.CalculateDefaultHitPosition(
                    head.transform, new Vector3(0f, -0.6f, 6f));
                Assert.That(position.y, Is.EqualTo(1.1f).Within(0.0001f));
                Assert.That(Vector3.Distance(new Vector3(position.x, 0f, position.z),
                    new Vector3(0f, 0f, 0f)), Is.EqualTo(6f).Within(0.0001f));
            }
            finally { Object.DestroyImmediate(head); }
        }

        [Test]
        public void DefaultPlacementStaysAheadOfRolledStereoCamera()
        {
            var head = new GameObject("Rolled stereo eye");
            try
            {
                head.transform.position = new Vector3(1f, 1.7f, -2f);
                head.transform.rotation = Quaternion.Euler(30f, 45f, 90f);
                var position = ARPlacementManager.CalculateDefaultHitPosition(
                    head.transform, new Vector3(0f, -0.6f, 6f));
                var horizontalDirection = Vector3.ProjectOnPlane(
                    position - head.transform.position, Vector3.up).normalized;
                var cameraHeading = Vector3.ProjectOnPlane(head.transform.forward, Vector3.up).normalized;
                Assert.That(Vector3.Dot(horizontalDirection, cameraHeading), Is.GreaterThan(0.999f));
                Assert.That(position.y, Is.EqualTo(1.1f).Within(0.0001f));
            }
            finally { Object.DestroyImmediate(head); }
        }

        [Test]
        public void StereoTrackingPreservesCenterEyeCalibrationAndMonoCameraPose()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.holoi.xr.holokit/Assets/Prefabs/HoloKit Camera Rig.prefab");
            Assert.That(prefab, Is.Not.Null);
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Assert.That(instance, Is.Not.Null);
            try
            {
                var manager = instance.GetComponentInChildren<HoloKitCameraManager>(true);
                Assert.That(manager, Is.Not.Null);
                var trackedPose = manager.StereoTrackedPose;
                var centerEye = new SerializedObject(manager).FindProperty("_centerEyePose").objectReferenceValue as Transform;
                Assert.That(trackedPose, Is.Not.Null);
                Assert.That(centerEye, Is.Not.Null);
                Assert.That(centerEye.parent, Is.EqualTo(trackedPose));

                var monoPose = manager.transform.position;
                centerEye.localPosition = new Vector3(0.02f, 0.06f, -0.09f);
                var trackingPosition = new Vector3(1f, 1.5f, -2f);
                var trackingRotation = Quaternion.Euler(10f, 40f, 5f);
                trackedPose.SetPositionAndRotation(trackingPosition, trackingRotation);

                var expectedEyePosition = trackingPosition + trackingRotation * centerEye.localPosition;
                Assert.That(Vector3.Distance(centerEye.position, expectedEyePosition), Is.LessThan(0.0001f));
                Assert.That(centerEye.localPosition, Is.EqualTo(new Vector3(0.02f, 0.06f, -0.09f)));
                Assert.That(manager.transform.position, Is.EqualTo(monoPose));
            }
            finally { Object.DestroyImmediate(instance); }
        }
    }
}
