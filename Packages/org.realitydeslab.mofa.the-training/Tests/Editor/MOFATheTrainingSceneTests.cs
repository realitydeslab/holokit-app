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

        [Test]
        public void AiAvatarHasAnIosVisibleMaterial()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/org.realitydeslab.mofa.the-training/Assets/Prefabs/Mofa AI Player.prefab");
            Assert.That(prefab, Is.Not.Null);
            var player = prefab.GetComponent("MofaAIPlayer");
            Assert.That(player, Is.Not.Null);
            var fallback = new SerializedObject(player).FindProperty("_iosAvatarFallbackMaterial").objectReferenceValue as Material;
            Assert.That(fallback, Is.Not.Null);
            Assert.That(fallback.shader.name, Is.EqualTo("Universal Render Pipeline/Unlit"));
            var weapon = new SerializedObject(player).FindProperty("_aiWeaponPrefab").objectReferenceValue as GameObject;
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
    }
}
