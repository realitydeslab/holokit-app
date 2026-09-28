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
                            resolved[guid] = exists = GuidAssetExists(guid);
                        if (!exists) missing.Add(path + ":" + lineNumber + " -> " + guid);
                    }
                }
            }
            Assert.That(missing, Is.Empty,
                "Missing asset GUIDs in playable product dependencies (" + paths.Length + " files checked):\n" +
                string.Join("\n", missing));
        }

        // Shader Graph and VFX Graph files embed defaults as JSON: \"guid\":\"<32 hex>\".
        private static readonly Regex JsonGuidReference = new(@"guid\\?""\s*:\s*\\?""([0-9a-f]{32})", RegexOptions.Compiled);

        // Missing graph references that predate the upgrade and have no recoverable original.
        // The test fails on any new missing reference and on entries that no longer need listing.
        private const string KnownMissingGraphReferences =
            "Packages/com.holoi.library.holokit-app-lib/Tests/Editor/KnownMissingGraphReferences.txt";

        [Test]
        public void PlayableProductGraphReferencesResolveOrAreKnown()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            var seeds = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path)
                .Concat(new[] { AssetDatabase.GetAssetPath(settings) })
                .Where(path => !string.IsNullOrEmpty(path)).ToArray();
            var graphs = AssetDatabase.GetDependencies(seeds, true)
                .Where(path => path.EndsWith(".shadergraph", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".vfx", StringComparison.OrdinalIgnoreCase))
                .Distinct().OrderBy(path => path, StringComparer.Ordinal).ToArray();
            Assert.That(graphs, Is.Not.Empty, "No Shader Graph or VFX Graph found; the dependency scan is broken.");

            var missing = new SortedSet<string>(StringComparer.Ordinal);
            var resolved = new Dictionary<string, bool>();
            foreach (var path in graphs)
            foreach (var line in File.ReadLines(path))
            {
                // The Shader Graph preview mesh is editor-only and does not affect rendering.
                if (line.Contains("m_SerializedMesh")) continue;
                // Shader Graph stores references as escaped JSON; VFX Graph as YAML.
                foreach (Match match in JsonGuidReference.Matches(line).Concat(GuidReference.Matches(line)))
                {
                    var guid = match.Groups[1].Value;
                    if (guid.StartsWith("0000000000000000", StringComparison.Ordinal)) continue;
                    if (!resolved.TryGetValue(guid, out var exists))
                        resolved[guid] = exists = GuidAssetExists(guid);
                    if (!exists) missing.Add(path + " -> " + guid);
                }
            }

            var known = new HashSet<string>(File.ReadAllLines(KnownMissingGraphReferences)
                .Select(line => line.Trim()).Where(line => line.Length > 0 && !line.StartsWith("#")), StringComparer.Ordinal);
            var inThisProduct = new HashSet<string>(graphs, StringComparer.Ordinal);
            var added = missing.Where(entry => !known.Contains(entry)).ToList();
            var fixedButListed = known.Where(entry => inThisProduct.Contains(entry.Split(" -> ")[0]) && !missing.Contains(entry)).ToList();
            Assert.That(added, Is.Empty,
                "New missing assets in graphs (" + graphs.Length + " checked); they render with a fallback:\n" + string.Join("\n", added));
            Assert.That(fixedButListed, Is.Empty,
                "These now resolve; remove them from " + KnownMissingGraphReferences + ":\n" + string.Join("\n", fixedButListed));
        }

        private static bool GuidAssetExists(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return false;

            // A local package can retain an obsolete GUID mapping in the Library
            // after its source .meta is removed. Resolve the on-disk path too.
            if (assetPath.StartsWith("Assets/", StringComparison.Ordinal))
            {
                var physicalPath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
                return File.Exists(physicalPath) || Directory.Exists(physicalPath);
            }

            if (assetPath.StartsWith("Packages/", StringComparison.Ordinal))
            {
                var package = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
                if (package != null && !string.IsNullOrEmpty(package.resolvedPath))
                {
                    var prefix = "Packages/" + package.name + "/";
                    if (assetPath.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        var physicalPath = Path.Combine(package.resolvedPath, assetPath.Substring(prefix.Length));
                        return File.Exists(physicalPath) || Directory.Exists(physicalPath);
                    }
                }
            }

            return AssetDatabase.LoadMainAssetAtPath(assetPath) != null;
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
