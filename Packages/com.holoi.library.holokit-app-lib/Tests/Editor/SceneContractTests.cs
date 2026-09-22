using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.AssetFoundation;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloKit.Tests
{
    /// <summary>Inherited by each game's own package-local scene test fixture.</summary>
    [Category("HoloKit.SceneContract")]
    public abstract class SceneContractTests
    {
        protected abstract string ScenePath { get; }
        private Scene _scene;
        private SceneSetup[] _previous;
        private string PackagePath => string.Join("/", ScenePath.Split('/').Take(2));
        private IEnumerable<GameObject> Objects => _scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).Select(t => t.gameObject);
        private RealityManager Manager => Objects.SelectMany(go => go.GetComponents<RealityManager>()).Single();

        [OneTimeSetUp]
        public void OpenScene()
        {
            _previous = EditorSceneManager.GetSceneManagerSetup();
            if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty))
                Assert.Fail("Save edited scenes before running scene contracts.");
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath), Is.Not.Null, "Missing product scene: " + ScenePath);
            _scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        [OneTimeTearDown]
        public void RestoreScene()
        {
            if (_previous != null && _previous.Length > 0 && _previous.All(s => !string.IsNullOrEmpty(s.path)))
                EditorSceneManager.RestoreSceneManagerSetup(_previous);
            else
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void EverySceneComponentHasItsScript()
        {
            var broken = Objects.Where(go => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0);
            Assert.That(broken.Select(go => go.name), Is.Empty, "Missing scripts in " + ScenePath);
        }

        [Test]
        public void SerializedSceneReferencesResolve()
        {
            var missing = new List<string>();
            foreach (var go in Objects)
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component == null) continue;
                    using var serialized = new SerializedObject(component);
                    var property = serialized.GetIterator();
                    while (property.Next(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference &&
                            property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                            missing.Add(go.name + ": " + component.GetType().Name + "." + property.propertyPath);
                }
            Assert.That(missing, Is.Empty, string.Join("\n", missing));
        }

        [Test]
        public void RealityManagerHasNetworkIdentityAndConfiguration()
        {
            var managers = Objects.SelectMany(go => go.GetComponents<RealityManager>()).ToArray();
            Assert.That(managers, Has.Length.EqualTo(1), "A playable reality needs exactly one manager.");
            Assert.That(Manager.GetComponent<NetworkObject>(), Is.Not.Null);
            Assert.That(Manager.GetComponent<RealityConfiguration>(), Is.Not.Null);
        }

        [Test]
        public void SpawnablePrefabsHaveNetworkIdentity()
        {
            var config = Manager.GetComponent<RealityConfiguration>();
            Assert.That(config.NetworkPrefabs, Is.Not.Null);
            var prefabs = new List<GameObject>(config.NetworkPrefabs);
            Assert.That(prefabs, Is.Unique, "Duplicate network prefab registration.");
            if (config.PlayerPrefab != null) prefabs.Add(config.PlayerPrefab);
            foreach (var prefab in prefabs)
            {
                Assert.That(prefab, Is.Not.Null, "Missing spawnable prefab.");
                Assert.That(prefab.GetComponent<NetworkObject>(), Is.Not.Null, prefab.name);
                foreach (var transform in prefab.GetComponentsInChildren<Transform>(true))
                    Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject), Is.Zero, prefab.name + "/" + transform.name);
            }
        }

        [Test]
        public void CatalogEntryResolvesItsSceneAndCover()
        {
            var entries = AssetDatabase.FindAssets("t:Reality", new[] { PackagePath })
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Reality>)
                .Where(r => r != null && Path.GetFileNameWithoutExtension(r.Scene.SceneName) == Path.GetFileNameWithoutExtension(ScenePath)).ToArray();
            Assert.That(entries, Has.Length.EqualTo(1), "Exactly one catalog entry must address this scene.");
            Assert.That(entries[0].BundleId, Is.Not.Null.And.Not.Empty);
            Assert.That(entries[0].DisplayName, Is.Not.Null.And.Not.Empty);
            Assert.That(entries[0].ThumbnailPrefab, Is.Not.Null, "The product gallery needs a cover.");
            Assert.That(entries[0].RealityEntranceOptions, Is.Not.Null.And.Not.Empty, "The reality needs an entrance action.");
        }

        [Test]
        public void GameUiPanelsResolveWithoutDuplicateKeys()
        {
            var config = Manager.GetComponent<RealityConfiguration>();
            Assert.That(config.UIPanelPrefabs, Is.Not.Null);
            Assert.That(config.UIPanelPrefabs.All(panel => panel != null), Is.True, "Missing game UI panel.");
            var keys = config.UIPanelPrefabs.Select(panel => panel.UIPanelName).ToArray();
            Assert.That(keys.All(key => !string.IsNullOrWhiteSpace(key)), Is.True);
            Assert.That(keys, Is.Unique);
            Assert.That(config.UIRealitySettingTabPrefabs, Is.Not.Null);
            Assert.That(config.UIRealitySettingTabPrefabs.All(tab => tab != null), Is.True, "Missing game settings tab.");
        }

        protected void AssertRequiredReferences(params string[] fieldNames)
        {
            using var serialized = new SerializedObject(Manager);
            foreach (var name in fieldNames)
            {
                var property = serialized.FindProperty(name);
                Assert.That(property, Is.Not.Null, "Missing expected gameplay field: " + name);
                if (property.isArray)
                {
                    Assert.That(property.arraySize, Is.GreaterThan(0), name);
                    for (var i = 0; i < property.arraySize; i++)
                        Assert.That(property.GetArrayElementAtIndex(i).objectReferenceValue, Is.Not.Null, name + "[" + i + "]");
                }
                else
                    Assert.That(property.objectReferenceValue, Is.Not.Null, name);
            }
        }

        protected void AssertComponentReferences(string componentName, params string[] fieldNames)
        {
            var components = Objects.SelectMany(go => go.GetComponents<MonoBehaviour>())
                .Where(c => c != null && c.GetType().Name == componentName).ToArray();
            Assert.That(components, Is.Not.Empty, "Missing component: " + componentName);
            foreach (var component in components)
            {
                using var serialized = new SerializedObject(component);
                foreach (var name in fieldNames)
                {
                    var property = serialized.FindProperty(name);
                    Assert.That(property, Is.Not.Null, componentName + "." + name);
                    Assert.That(property.objectReferenceValue, Is.Not.Null, component.name + ": " + componentName + "." + name);
                }
            }
        }

        [Test]
        public void CameraDirectionFeedersHaveMatchingVfxParameters()
        {
            foreach (var component in Objects.SelectMany(go => go.GetComponents<MonoBehaviour>())
                         .Where(c => c != null && c.GetType().Name == "CameraForwardVfxFeeder"))
            {
                var effect = component.GetComponent<UnityEngine.VFX.VisualEffect>();
                Assert.That(effect, Is.Not.Null, component.name);
                Assert.That(effect.HasVector3("CameraForward"), Is.True, component.name + " graph has no CameraForward Vector3.");
            }
        }
    }
}
