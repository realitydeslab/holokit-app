using HoloKit.Tests;
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
        }
    }
}
