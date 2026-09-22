using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoloKit.Tests
{
    public sealed class SerializedAssetReferenceTests
    {
        private static readonly Regex GuidReference = new(@"\bguid: ([0-9a-f]{32})\b", RegexOptions.Compiled);

        [Test]
        public void PlayableProductAssetGuidsResolve()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            var seeds = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path)
                .Concat(new[] { AssetDatabase.GetAssetPath(settings) })
                .Where(path => !string.IsNullOrEmpty(path)).ToArray();
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { ".unity", ".prefab", ".asset", ".controller", ".overrideController", ".anim", ".mat" };
            var paths = AssetDatabase.GetDependencies(seeds, true).Concat(seeds)
                .Where(path => extensions.Contains(Path.GetExtension(path)))
                .Distinct().OrderBy(path => path, StringComparer.Ordinal).ToArray();
            var missing = new List<string>();
            var resolved = new Dictionary<string, bool>();
            foreach (var path in paths)
            {
                if (!File.Exists(path)) continue;
                var lineNumber = 0;
                foreach (var line in File.ReadLines(path))
                {
                    lineNumber++;
                    foreach (Match match in GuidReference.Matches(line))
                    {
                        var guid = match.Groups[1].Value;
                        if (guid.StartsWith("0000000000000000", StringComparison.Ordinal)) continue;
                        if (!resolved.TryGetValue(guid, out var exists))
                            resolved[guid] = exists = !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));
                        if (!exists) missing.Add(path + ":" + lineNumber + " -> " + guid);
                    }
                }
            }
            Assert.That(missing, Is.Empty,
                "Missing asset GUIDs in playable product dependencies (" + paths.Length + " files checked):\n" +
                string.Join("\n", missing));
        }

        [Test]
        public void PlayableProductPrefabsHaveNoMissingScripts()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            var seeds = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path)
                .Concat(new[] { AssetDatabase.GetAssetPath(settings) }).ToArray();
            var paths = AssetDatabase.GetDependencies(seeds, true)
                .Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));
            var missing = new List<string>();
            foreach (var path in paths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                foreach (var transform in prefab.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject) != 0)
                        missing.Add(path + ": " + transform.name);
            }
            Assert.That(missing, Is.Empty, string.Join("\n", missing));
        }
    }
}
