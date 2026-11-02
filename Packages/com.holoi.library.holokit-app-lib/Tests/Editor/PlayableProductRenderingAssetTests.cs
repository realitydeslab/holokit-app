using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>
    /// Checks every material, shader and VFX asset that the product's enabled scenes and
    /// configuration depend on. Covers content that only appears after interaction, which
    /// a Play Mode walkthrough does not reach.
    /// </summary>
    public sealed class PlayableProductRenderingAssetTests
    {
        // Passes URP draws. Legacy built-in passes (ForwardBase, Always, Vertex...) render
        // with URP's magenta error material instead.
        private static readonly HashSet<string> UrpDrawPasses = new(StringComparer.Ordinal)
            { "", "SRPDefaultUnlit", "UniversalForward", "UniversalForwardOnly", "UniversalGBuffer", "Universal2D" };

        private static string[] ProductDependencies()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            var seeds = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path)
                .Concat(new[] { AssetDatabase.GetAssetPath(settings) })
                .Where(path => !string.IsNullOrEmpty(path)).ToArray();
            return AssetDatabase.GetDependencies(seeds, true).Distinct().OrderBy(p => p, StringComparer.Ordinal).ToArray();
        }

        // A reality switches to its own URP asset at runtime (RealityConfiguration.UrpAsset), but the player build keeps
        // shader variants by the default URP asset's keyword prefiltering only. When the default asset has main light
        // shadows on, the build drops every lit shader's shadows-off variant. An asset with main light shadows off then
        // asks for that variant, and Metal pairs the programs of two others: "Fragment input(s) user(INTERP4) mismatching
        // vertex shader output" for SH_Meebits, and the MOFA Training opponent was not drawn (iPhone 15 Pro, 2026-11-02).
        // A reality that draws no shadows keeps shadows supported and sets the shadow distance to 0 instead.
        [Test]
        public void RealityUrpAssetsMatchTheDefaultAssetsMainLightShadowSupport()
        {
            var defaultAsset = GraphicsSettings.defaultRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            Assert.That(defaultAsset, Is.Not.Null, "The default render pipeline is not a URP asset.");
            var urpAssets = ProductDependencies().Where(p => p.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                .Select(AssetDatabase.LoadMainAssetAtPath).OfType<UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset>().ToArray();
            Assert.That(urpAssets, Is.Not.Empty, "No URP asset found; the dependency scan is broken.");
            var mismatched = urpAssets.Where(asset => asset.supportsMainLightShadows != defaultAsset.supportsMainLightShadows)
                .Select(AssetDatabase.GetAssetPath).ToArray();
            Assert.That(mismatched, Is.Empty,
                $"The default URP asset has main light shadows {(defaultAsset.supportsMainLightShadows ? "on" : "off")}; these differ:\n" +
                string.Join("\n", mismatched));
        }

        [Test]
        public void PlayableProductMaterialsUseWorkingUrpShaders()
        {
            var problems = new List<string>();
            var materials = ProductDependencies().Where(p => p.EndsWith(".mat", StringComparison.OrdinalIgnoreCase)).ToArray();
            Assert.That(materials, Is.Not.Empty, "No materials found; the dependency scan is broken.");
            var checkedShaders = new Dictionary<Shader, string>();
            foreach (var path in materials)
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null) { problems.Add(path + ": material does not load"); continue; }
                var shader = material.shader;
                if (shader == null || shader.name == "Hidden/InternalErrorShader")
                {
                    problems.Add(path + ": shader is missing");
                    continue;
                }
                if (!checkedShaders.TryGetValue(shader, out var shaderProblem))
                    checkedShaders[shader] = shaderProblem = ShaderProblem(shader);
                if (shaderProblem != null) problems.Add($"{path}: shader '{shader.name}' {shaderProblem}");
            }
            Assert.That(problems, Is.Empty,
                $"Rendering problems in {materials.Length} product materials:\n" + string.Join("\n", problems));
        }

        [Test]
        public void PlayableProductShadersCompile()
        {
            var problems = new List<string>();
            var shaders = ProductDependencies()
                .Where(p => p.EndsWith(".shader", StringComparison.OrdinalIgnoreCase) ||
                            p.EndsWith(".shadergraph", StringComparison.OrdinalIgnoreCase)).ToArray();
            Assert.That(shaders, Is.Not.Empty, "No shaders found; the dependency scan is broken.");
            foreach (var path in shaders)
            {
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader == null) { problems.Add(path + ": shader does not load"); continue; }
                var problem = ShaderProblem(shader);
                if (problem != null) problems.Add($"{path}: {problem}");
            }
            Assert.That(problems, Is.Empty,
                $"Problems in {shaders.Length} product shaders:\n" + string.Join("\n", problems));
        }

        [Test]
        public void PlayableProductVisualEffectsLoad()
        {
            var effects = ProductDependencies().Where(p => p.EndsWith(".vfx", StringComparison.OrdinalIgnoreCase)).ToArray();
            var problems = effects.Where(p => AssetDatabase.LoadAssetAtPath<VisualEffectAsset>(p) == null)
                .Select(p => p + ": VFX asset does not load").ToList();
            Assert.That(problems, Is.Empty, string.Join("\n", problems));
        }

        private static string ProblemFromMessages(Shader shader)
        {
            var errors = ShaderUtil.GetShaderMessages(shader)
                .Where(m => m.severity == ShaderCompilerMessageSeverity.Error)
                .Select(m => m.message.Trim()).Distinct().Take(2).ToArray();
            return errors.Length == 0 ? null : "has compile errors: " + string.Join(" | ", errors);
        }

        private static string ShaderProblem(Shader shader)
        {
            if (ShaderUtil.ShaderHasError(shader))
                return ProblemFromMessages(shader) ?? "has compile errors";
            if (!shader.isSupported) return "is not supported on this graphics API";
            if (shader.name.StartsWith("Hidden/", StringComparison.Ordinal)) return null;
            var tag = new ShaderTagId("LightMode");
            for (var pass = 0; pass < shader.passCount; pass++)
                if (UrpDrawPasses.Contains(shader.FindPassTagValue(pass, tag).name)) return null;
            return "has no pass URP draws (legacy built-in shader renders magenta)";
        }
    }
}
