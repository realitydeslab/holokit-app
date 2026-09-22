using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Read-only scene/reference checks callable through Unity CLI.</summary>
public static class HoloKitUpgradeAudit
{
    [Serializable] public class Issue
    {
        public string asset;
        public string context;
        public string kind;
        public string detail;
    }

    [Serializable] public class Report
    {
        public string unityVersion;
        public string buildTarget;
        public int scenesChecked;
        public int componentsChecked;
        public List<Issue> issues = new();
    }

    public static void Run()
    {
        var report = new Report { unityVersion = Application.unityVersion, buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString() };
        var setup = EditorSceneManager.GetSceneManagerSetup();
        if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty))
            throw new InvalidOperationException("Save edited scenes before running the audit.");
        try
        {
            foreach (var entry in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                if (!File.Exists(entry.path))
                {
                    Add(report, entry.path, "", "missing-scene", entry.guid.ToString());
                    continue;
                }
                var scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
                report.scenesChecked++;
                foreach (var root in scene.GetRootGameObjects())
                    foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                        Inspect(report, entry.path, transform.gameObject);
            }
            var planeScripts = new[] {
                "Packages/com.holoi.library.arux/Assets/Prefabs/ARPlanes/_AR Feathered Plane/ARFeatheredPlaneMeshVisualizer.cs",
                "Packages/com.holoi.library.arux/Assets/Prefabs/ARPlanes/_AR Shadowed Plane/ARShadowedPlane.cs"
            };
            foreach (var path in planeScripts)
                if (AssetDatabase.LoadAssetAtPath<MonoScript>(path)?.GetClass() == null)
                    Add(report, path, "", "uncompiled-script", "MonoScript.GetClass returned null");
        }
        finally
        {
            // Batch startup can contain an untitled scene that cannot be restored.
            if (setup.Length > 0 && setup.All(s => !string.IsNullOrEmpty(s.path)))
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            else
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
        var output = Environment.GetEnvironmentVariable("HOLOKIT_AUDIT_OUTPUT") ?? "Logs/upgrade-audit.json";
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
        File.WriteAllText(output, JsonUtility.ToJson(report, true));
        Debug.Log($"[HoloKitUpgradeAudit] {report.scenesChecked} scenes, {report.componentsChecked} components, {report.issues.Count} issues. Report: {output}");
        if (report.issues.Count > 0)
            throw new InvalidOperationException($"Upgrade audit found {report.issues.Count} issues; see {output}");
    }

    private static void Inspect(Report report, string asset, GameObject go)
    {
        var hierarchy = go.name;
        for (var parent = go.transform.parent; parent != null; parent = parent.parent)
            hierarchy = parent.name + "/" + hierarchy;
        var missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        if (missing > 0) Add(report, asset, hierarchy, "missing-script", missing.ToString());
        foreach (var component in go.GetComponents<Component>())
        {
            if (component == null) continue;
            report.componentsChecked++;
            using var serialized = new SerializedObject(component);
            var property = serialized.GetIterator();
            while (property.Next(true))
                if (property.propertyType == SerializedPropertyType.ObjectReference &&
                    property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                    Add(report, asset, hierarchy, "missing-object-reference", component.GetType().FullName + "." + property.propertyPath);
        }
    }

    private static void Add(Report report, string asset, string context, string kind, string detail)
        => report.issues.Add(new Issue { asset = asset, context = context, kind = kind, detail = detail });
}
