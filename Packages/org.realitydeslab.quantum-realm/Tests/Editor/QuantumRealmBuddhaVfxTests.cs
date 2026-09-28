using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RealityDesignLab.QuantumRealm;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    public sealed class QuantumRealmBuddhaVfxTests
    {
        private const string BuddhaGroupPrefab = "Packages/org.realitydeslab.quantum-realm/Assets/Prefabs/BuddhaGroup.prefab";

        // VFX graphs that show no dissolve progress while a hand hovers. "Particles 1" has lacked the
        // Process property since the App Store build; adding it is an art change in the VFX Graph editor.
        private static readonly HashSet<string> KnownWithoutProcess = new() { "Particles 1" };

        [Test]
        public void EveryBuddhaVfxExposesThePropertiesItsControllerDrives()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BuddhaGroupPrefab);
            Assert.That(prefab, Is.Not.Null);
            var problems = new List<string>();
            var buddhas = prefab.GetComponentsInChildren<BuddhaController>(true);
            Assert.That(buddhas, Has.Length.EqualTo(10));
            foreach (var buddha in buddhas)
            {
                // BuddhaController writes these every frame while a hand hovers; Process is optional.
                var vfx = new SerializedObject(buddha).FindProperty("_vfx").objectReferenceValue as VisualEffect;
                if (vfx == null || vfx.visualEffectAsset == null)
                {
                    problems.Add($"{buddha.name}: no VisualEffect asset");
                    continue;
                }
                var asset = vfx.visualEffectAsset.name;
                if (vfx.HasFloat("Process") == KnownWithoutProcess.Contains(asset))
                    problems.Add(KnownWithoutProcess.Contains(asset)
                        ? $"{buddha.name} ({asset}) now has 'Process'; remove it from KnownWithoutProcess"
                        : $"{buddha.name} ({asset}): no float 'Process', so hovering shows no progress");
                if (!vfx.HasVector3("Hand Position")) problems.Add($"{buddha.name} ({vfx.visualEffectAsset.name}): no Vector3 'Hand Position'");
                if (!vfx.HasBool("Interactable")) problems.Add($"{buddha.name} ({vfx.visualEffectAsset.name}): no bool 'Interactable'");
            }
            Assert.That(problems, Is.Empty, string.Join("\n", problems));
        }
    }
}
