#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Holoi.Library.HoloKitAppLib.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Opt-in frame-time measurement of the reality gallery (HOLOKIT_GALLERY_PERF=1): idle on the first room, then a
    /// scroll through every room. Fails when the gallery idles below 60 fps or more than 2% of the scroll frames are
    /// hitches: iOS runs an app at 30 fps unless it asks for more, and the gallery did (2026-11-02, iPhone 15 Pro: every
    /// frame 33.3 ms while the CPU needed 6.4 ms and the GPU 7.5 ms).
    /// For investigation: HOLOKIT_GALLERY_PERF_FPS sets Application.targetFrameRate first (-1 = the platform default),
    /// and HOLOKIT_GALLERY_PERF_OFF switches parts off before measuring, comma separated:
    /// vfx, particles, animators, skinned, offscreen, shadows, hdr, postfx, scale75, canvas.
    /// </summary>
    public sealed class DeviceGalleryPerfTests : MonoBehaviour
    {
        private const float Budget = 1000f / 60f;
        private const float MaxHitchShare = .02f;

        [Serializable] private sealed class Phase
        {
            public string name;
            public int frames;
            public float meanMs, p50Ms, p95Ms, p99Ms, maxMs;
            public int hitches; // frames longer than 1.5 frame budgets
            public int gcCollections;
            public float cpuMainP50, cpuMainP95, cpuRenderP50, cpuRenderP95, gpuP50, gpuP95;
        }

        [Serializable] private sealed class Report
        {
            public string device, unityVersion, off, fpsOverride, screen;
            public int targetFrameRate, vSyncCount, refreshRate;
            public bool frameTiming;
            public int rooms, renderers, skinnedMeshes, animators, visualEffects, vfxAliveParticles, particleSystems, particles, lights, shadowLights;
            public bool complete;
            public List<string> errors = new();
            public List<Phase> phases = new();
        }

        private sealed class Samples
        {
            public readonly List<float> Intervals = new(), CpuMain = new(), CpuRender = new(), Gpu = new();
            public int Gc;
        }

        private readonly Report _report = new();
        private readonly FrameTiming[] _timing = new FrameTiming[1];
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            if (Environment.GetEnvironmentVariable("HOLOKIT_GALLERY_PERF") != "1") return;
            var runner = new GameObject("HoloKit Gallery Perf Test");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceGalleryPerfTests>();
        }

        private IEnumerator Start()
        {
            _directory = Path.Combine(Application.persistentDataPath, "HoloKitGalleryPerf");
            Directory.CreateDirectory(_directory);
            File.Delete(Path.Combine(_directory, "results.json"));
            _report.off = Environment.GetEnvironmentVariable("HOLOKIT_GALLERY_PERF_OFF") ?? "";
            _report.fpsOverride = Environment.GetEnvironmentVariable("HOLOKIT_GALLERY_PERF_FPS") ?? "";
            _report.device = SystemInfo.deviceModel;
            _report.unityVersion = Application.unityVersion;
            var run = Run();
            while (true)
            {
                bool more;
                try { more = run.MoveNext(); }
                catch (Exception exception) { _report.errors.Add(exception.ToString()); break; }
                if (!more) { _report.complete = true; break; }
                yield return run.Current;
            }
            Save();
            Debug.Log($"[GALLERY PERF] complete={_report.complete} off='{_report.off}' errors={_report.errors.Count} report={_directory}/results.json");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            HoloKitAppUIPanel_RealityGalleryPage page = null;
            for (var i = 0; i < 300 && page == null; i++)
            {
                page = FindAnyObjectByType<HoloKitAppUIPanel_RealityGalleryPage>();
                if (page == null) yield return new WaitForSecondsRealtime(.1f);
            }
            if (page == null) throw new InvalidOperationException("The reality gallery did not open within 30 s.");
            var type = typeof(HoloKitAppUIPanel_RealityGalleryPage);
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var rooms = (List<GameObject>)type.GetField("RoomList", flags).GetValue(page);
            var next = type.GetMethod("ScrollToNextRoom", flags);
            yield return new WaitForSecondsRealtime(3f); // loading hitches are not what the user feels while browsing

            if (int.TryParse(_report.fpsOverride, out var fps)) Application.targetFrameRate = fps;
            _report.targetFrameRate = Application.targetFrameRate;
            _report.vSyncCount = QualitySettings.vSyncCount;
            _report.refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;
            _report.screen = $"{Screen.width}x{Screen.height}";
            _report.frameTiming = FrameTimingManager.IsFeatureEnabled();
            Inventory(rooms);
            var off = new HashSet<string>(_report.off.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0));
            SwitchOff(off, rooms);
            Save();

            yield return Measure("idle room 1", 6f);
            var moves = new Samples();
            for (var room = 1; room < rooms.Count; room++)
            {
                if (off.Contains("offscreen")) ShowAround(rooms, room);
                next.Invoke(page, null);
                yield return Measure("move", .8f, moves);
                yield return Measure($"idle room {room + 1}", 1.2f);
            }
            var scroll = Stats("scroll moves", moves);
            _report.phases.Add(scroll);
            var idle = _report.phases[0];
            if (idle.p50Ms > Budget * 1.05f)
                _report.errors.Add($"The gallery idles at {1000f / idle.p50Ms:F0} fps (median frame {idle.p50Ms:F1} ms; targetFrameRate {_report.targetFrameRate}), below 60 fps.");
            if (scroll.hitches > MaxHitchShare * scroll.frames)
                _report.errors.Add($"{scroll.hitches} of {scroll.frames} frames while scrolling took longer than {1.5f * Budget:F0} ms (p95 {scroll.p95Ms:F1} ms).");
        }

        /// <summary>Records every frame for <paramref name="seconds"/>, as its own phase or into <paramref name="into"/>.</summary>
        private IEnumerator Measure(string name, float seconds, Samples into = null)
        {
            var samples = into ?? new Samples();
            var gc = GC.CollectionCount(0);
            yield return null;
            var end = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < end)
            {
                yield return null;
                samples.Intervals.Add(Time.unscaledDeltaTime * 1000f);
                FrameTimingManager.CaptureFrameTimings();
                if (FrameTimingManager.GetLatestTimings(1, _timing) > 0)
                {
                    samples.CpuMain.Add((float)_timing[0].cpuMainThreadFrameTime);
                    samples.CpuRender.Add((float)_timing[0].cpuRenderThreadFrameTime);
                    samples.Gpu.Add((float)_timing[0].gpuFrameTime);
                }
            }
            samples.Gc += GC.CollectionCount(0) - gc;
            if (into != null) yield break;
            _report.phases.Add(Stats(name, samples));
            Save();
        }

        private static Phase Stats(string name, Samples samples)
        {
            static List<float> Sort(List<float> values) => values.OrderBy(x => x).ToList();
            static float P(List<float> s, float q) => s.Count == 0 ? 0 : s[Mathf.Clamp(Mathf.CeilToInt(q * s.Count) - 1, 0, s.Count - 1)];
            var sorted = Sort(samples.Intervals);
            var main = Sort(samples.CpuMain);
            var render = Sort(samples.CpuRender);
            var gpu = Sort(samples.Gpu);
            return new Phase
            {
                name = name, frames = sorted.Count, meanMs = sorted.Count == 0 ? 0 : sorted.Average(), p50Ms = P(sorted, .5f),
                p95Ms = P(sorted, .95f), p99Ms = P(sorted, .99f), maxMs = sorted.Count == 0 ? 0 : sorted[^1],
                hitches = sorted.Count(x => x > 1.5f * Budget), gcCollections = samples.Gc,
                cpuMainP50 = P(main, .5f), cpuMainP95 = P(main, .95f), cpuRenderP50 = P(render, .5f), cpuRenderP95 = P(render, .95f),
                gpuP50 = P(gpu, .5f), gpuP95 = P(gpu, .95f)
            };
        }

        private void Inventory(List<GameObject> rooms)
        {
            var parts = rooms.Where(r => r != null).ToList();
            T[] All<T>() where T : Component => parts.SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
            _report.rooms = parts.Count;
            _report.renderers = All<Renderer>().Length;
            _report.skinnedMeshes = All<SkinnedMeshRenderer>().Length;
            _report.animators = All<Animator>().Length;
            var vfx = All<VisualEffect>();
            _report.visualEffects = vfx.Length;
            _report.vfxAliveParticles = vfx.Sum(v => v.aliveParticleCount);
            var particles = All<ParticleSystem>();
            _report.particleSystems = particles.Length;
            _report.particles = particles.Sum(p => p.particleCount);
            var lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            _report.lights = lights.Length;
            _report.shadowLights = lights.Count(l => l.isActiveAndEnabled && l.shadows != LightShadows.None);
        }

        private static void SwitchOff(HashSet<string> off, List<GameObject> rooms)
        {
            T[] All<T>() where T : Component => rooms.Where(r => r != null).SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
            if (off.Contains("vfx")) foreach (var v in All<VisualEffect>()) v.enabled = false;
            if (off.Contains("particles"))
                foreach (var p in All<ParticleSystem>())
                {
                    p.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                    if (p.TryGetComponent<ParticleSystemRenderer>(out var r)) r.enabled = false;
                }
            if (off.Contains("animators")) foreach (var a in All<Animator>()) a.enabled = false;
            if (off.Contains("skinned")) foreach (var s in All<SkinnedMeshRenderer>()) s.enabled = false;
            if (off.Contains("offscreen")) ShowAround(rooms, 0);
            var asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (asset != null)
            {
                if (off.Contains("shadows")) asset.shadowDistance = 0f;
                if (off.Contains("hdr")) asset.supportsHDR = false;
                if (off.Contains("scale75")) asset.renderScale = .75f;
            }
            if (off.Contains("postfx") && Camera.main != null && Camera.main.TryGetComponent<UniversalAdditionalCameraData>(out var data))
                data.renderPostProcessing = false;
            if (off.Contains("canvas")) foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None)) c.enabled = false;
        }

        private static void ShowAround(List<GameObject> rooms, int index)
        {
            for (var i = 0; i < rooms.Count; i++)
                if (rooms[i] != null) rooms[i].SetActive(Mathf.Abs(i - index) <= 1);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
