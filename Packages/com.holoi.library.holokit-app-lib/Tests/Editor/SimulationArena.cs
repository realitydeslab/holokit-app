using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Simulation;

namespace HoloKit.Tests
{
    /// <summary>
    /// The HoloKit test arena for XR Simulation: a 12 m x 12 m room with walls and a table, so a simulated
    /// person can walk a Quantum Realm circle or a MOFA duel (XR Simulation's default room only allows about
    /// 1.3 m of movement). <see cref="Use"/> selects it for the next Play session; <see cref="Restore"/> puts
    /// back the environment that was selected before, also after the Play Mode domain reload.
    /// </summary>
    public static class SimulationArena
    {
        public const string PrefabPath = "Packages/com.holoi.library.holokit-app-lib/Tests/Editor/Environments/HoloKit Test Arena.prefab";
        private const string PreferencesPath = "Assets/XR/UserSimulationSettings/Resources/XRSimulationPreferences.asset";
        private const string ActiveKey = "HoloKit.SimulationArena.Active";
        private const string PreviousKey = "HoloKit.SimulationArena.Previous";
        private const int EnvironmentLayer = 30;                                       // XRSimulationRuntimeSettings.environmentLayer
        private const string EnvironmentMaterialGuid = "950042044eb8b45cfb957653439b0be7"; // AR Foundation's simulation material

        /// <summary>Where the phone starts: the middle of the room, at a person's chest height, facing +Z.</summary>
        public static readonly Vector3 StartPosition = new(0f, 1.45f, 0f);
        private static readonly Bounds MovementBounds = new(new Vector3(0f, 1.4f, 0f), new Vector3(11.2f, 2.6f, 11.2f));

        public static GameObject Prefab => AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) ?? Build();

        public static void Use()
        {
            var (preferences, environment) = Preferences();
            if (!SessionState.GetBool(ActiveKey, false))
            {
                var previous = environment.objectReferenceValue;
                SessionState.SetString(PreviousKey, previous == null ? "" : AssetDatabase.GetAssetPath(previous));
                SessionState.SetBool(ActiveKey, true);
            }
            environment.objectReferenceValue = Prefab;
            environment.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(preferences);
        }

        public static void Restore()
        {
            if (!SessionState.GetBool(ActiveKey, false)) return;
            var (preferences, environment) = Preferences();
            var previous = SessionState.GetString(PreviousKey, "");
            environment.objectReferenceValue = previous == "" ? null : AssetDatabase.LoadAssetAtPath<GameObject>(previous);
            environment.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(preferences);
            SessionState.EraseBool(ActiveKey);
            SessionState.EraseString(PreviousKey);
        }

        private static (ScriptableObject, SerializedProperty) Preferences()
        {
            var preferences = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PreferencesPath);
            Assert.That(preferences, Is.Not.Null, $"This project has no XR Simulation preferences at {PreferencesPath}.");
            var environment = new SerializedObject(preferences).FindProperty("m_EnvironmentPrefab");
            Assert.That(environment, Is.Not.Null, "XR Simulation preferences no longer have m_EnvironmentPrefab.");
            return (preferences, environment);
        }

        /// <summary>Builds the arena prefab from primitives; delete the prefab to rebuild it after changing this.</summary>
        public static GameObject Build()
        {
            var scene = EditorSceneManager.NewPreviewScene();
            var root = new GameObject("HoloKit Test Arena");
            SceneManager.MoveGameObjectToScene(root, scene);
            try
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(EnvironmentMaterialGuid));
                Block(root, "Floor", new Vector3(0f, -0.01f, 0f), new Vector3(12f, 0.02f, 12f), material);
                Block(root, "Wall North", new Vector3(0f, 1.5f, 6f), new Vector3(12f, 3f, 0.1f), material);
                Block(root, "Wall South", new Vector3(0f, 1.5f, -6f), new Vector3(12f, 3f, 0.1f), material);
                Block(root, "Wall East", new Vector3(6f, 1.5f, 0f), new Vector3(0.1f, 3f, 12f), material);
                Block(root, "Wall West", new Vector3(-6f, 1.5f, 0f), new Vector3(0.1f, 3f, 12f), material);
                Block(root, "Table", new Vector3(3f, 0.375f, 3.5f), new Vector3(1.2f, 0.75f, 0.8f), material);
                var light = new GameObject("Light").AddComponent<Light>();
                light.transform.SetParent(root.transform, false);
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                light.type = LightType.Directional;

                // SimulationEnvironment is internal to AR Foundation; add it by type and set it through serialization.
                var type = typeof(SimulationCameraPoseProvider).Assembly.GetType("UnityEngine.XR.Simulation.SimulationEnvironment", true);
                var settings = new SerializedObject(root.AddComponent(type));
                settings.FindProperty("m_CameraStartingPose.position").vector3Value = StartPosition;
                settings.FindProperty("m_CameraStartingPose.rotation").quaternionValue = Quaternion.identity;
                settings.FindProperty("m_CameraMovementBounds").boundsValue = MovementBounds;
                settings.FindProperty("m_DefaultViewPose.position").vector3Value = new Vector3(0f, 4f, -8f);
                settings.FindProperty("m_DefaultViewPose.rotation").quaternionValue = Quaternion.Euler(25f, 0f, 0f);
                settings.ApplyModifiedPropertiesWithoutUndo();

                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    transform.gameObject.layer = EnvironmentLayer;
                Directory.CreateDirectory(Path.GetDirectoryName(FileUtil.GetPhysicalPath(PrefabPath)));
                return PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }

        private static void Block(GameObject root, string name, Vector3 position, Vector3 size, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(root.transform, false);
            block.transform.localPosition = position;
            block.transform.localScale = size;
            if (material != null) block.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}
