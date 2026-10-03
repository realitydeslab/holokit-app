using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;

namespace HoloKit.Tests
{
    /// <summary>
    /// Plays every catalog scene of the open product in the Editor with XR Simulation,
    /// as Player and Spectator. Records runtime errors, missing scripts, unsupported
    /// shaders, empty VFX and a camera render per case. It does not simulate hand input,
    /// remote peers, stereo optics or native iOS plugins.
    /// </summary>
    [Explicit("Minutes long; run with --filter EditorScenePlaythroughTests.")]
    [Category("EditorPlaythrough")]
    public sealed class EditorScenePlaythroughTests
    {
        private const float SceneLoadTimeout = 30f;
        private const float TrackingTimeout = 30f;
        private const float WarmupSeconds = 3f;
        private const int RenderWidth = 540;
        private const int RenderHeight = 960;

        [Serializable] private sealed class CaseResult
        {
            public string scene;
            public string role;
            public string arState;
            public bool hostListening;
            public bool multiplayerManagerSpawned;
            public bool returnedToGallery;
            public bool hoverStartButton;
            public bool startedWithoutHand;
            public int renderers;
            public int missingScripts;
            public float magentaFraction;
            public float litFraction;
            public string render;
            public string camera;
            public bool cameraBackground;
            public List<string> errors = new();
            public List<string> warnings = new();
            // Errors raised inside AR Foundation's XR Simulation itself; Editor-only, not app code.
            public List<string> simulationErrors = new();
        }

        [Serializable] private sealed class Report
        {
            public string product;
            public string unityVersion;
            public bool complete;
            public List<string> startupErrors = new();
            public List<CaseResult> cases = new();
        }

        private Report _report;
        private CaseResult _current;
        private string _directory;

        [UnityTest]
        public IEnumerator EveryCatalogSceneRendersWithoutErrors()
        {
            var startScene = EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled);
            Assert.That(startScene, Is.Not.Null, "The product has no enabled build scene.");
            EditorSceneManager.OpenScene(startScene.path, OpenSceneMode.Single);
            yield return new EnterPlayMode();

            // Entering Play Mode reloads the domain, so per-run state starts here.
            LogAssert.ignoreFailingMessages = true;
            _directory = OutputDirectory();
            _report = new Report { unityVersion = Application.unityVersion, product = Application.identifier };
            Application.logMessageReceivedThreaded += OnLog;
            try
            {
                var run = Run();
                while (true)
                {
                    bool more;
                    try { more = run.MoveNext(); }
                    catch (Exception exception) { Errors.Add(exception.ToString()); break; }
                    if (!more) { _report.complete = true; break; }
                    yield return run.Current;
                }
            }
            finally
            {
                Application.logMessageReceivedThreaded -= OnLog;
                Save();
            }
            yield return new ExitPlayMode();

            // Exiting reloads the domain again; read back what the run saved.
            _directory = OutputDirectory();
            Assert.That(File.Exists(ReportPath), Is.True, "Playthrough wrote no report: " + ReportPath);
            var report = JsonUtility.FromJson<Report>(File.ReadAllText(ReportPath));
            var failures = report.cases.Where(c => c.errors.Count > 0)
                .Select(c => $"{c.scene}/{c.role}: {c.errors.Count} error(s), first: {Head(c.errors[0])}").ToList();
            failures.AddRange(report.startupErrors.Select(e => "startup: " + Head(e)));
            Assert.That(report.complete, Is.True, "Playthrough did not finish. Report: " + ReportPath);
            Assert.That(failures, Is.Empty, $"Report: {ReportPath}\n" + string.Join("\n", failures));
        }

        private static string OutputDirectory()
        {
            var directory = Environment.GetEnvironmentVariable("HOLOKIT_PLAYTHROUGH_OUTPUT");
            if (string.IsNullOrEmpty(directory))
                directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs", "EditorPlaythrough"));
            Directory.CreateDirectory(directory);
            return directory;
        }

        private IEnumerator Run()
        {
            var app = HoloKitApp.Instance;
            for (var t = 0f; app == null && t < SceneLoadTimeout; t += Time.unscaledDeltaTime)
            {
                yield return null;
                app = HoloKitApp.Instance;
            }
            if (app == null) throw new InvalidOperationException("App bootstrap did not create HoloKitApp.");
            yield return new WaitForSecondsRealtime(2);
            var gallery = SceneManager.GetActiveScene().name;
            var realities = app.GlobalSettings.GetAllRealities().ToArray();
            if (realities.Length == 0) throw new InvalidOperationException("The product catalog is empty.");
            var only = Environment.GetEnvironmentVariable("HOLOKIT_PLAYTHROUGH_SCENE");
            if (!string.IsNullOrEmpty(only)) realities = realities.Where(r => r.Scene.SceneName == only).ToArray();

            foreach (var reality in realities)
            foreach (var role in new[] { HoloKitAppPlayerType.Player, HoloKitAppPlayerType.Spectator })
            {
                _current = new CaseResult { scene = reality.Scene.SceneName, role = role.ToString() };
                _report.cases.Add(_current);
                Debug.Log($"[PLAYTHROUGH] ENTER {_current.scene} role={role}");
                app.CurrentReality = reality;
                app.EnterRealityAs(true, role);
                for (var t = 0f; SceneManager.GetActiveScene().name != _current.scene && t < SceneLoadTimeout; t += Time.unscaledDeltaTime)
                    yield return null;
                if (SceneManager.GetActiveScene().name != _current.scene)
                    Errors.Add("Scene did not load; active scene is " + SceneManager.GetActiveScene().name);
                for (var t = 0f; ARSession.state != ARSessionState.SessionTracking && t < TrackingTimeout; t += Time.unscaledDeltaTime)
                    yield return null;
                _current.arState = ARSession.state.ToString();
                if (ARSession.state != ARSessionState.SessionTracking)
                    Errors.Add("XR Simulation did not reach SessionTracking: " + _current.arState);
                yield return EditorAppDriver.RenderMainCameraFor(WarmupSeconds);

                if (app.RealityManager == null) Errors.Add("Reality manager did not initialize.");
                _current.hostListening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsListening;
                if (!_current.hostListening) Errors.Add("Local network host did not start.");
                _current.multiplayerManagerSpawned = app.MultiplayerManager != null && app.MultiplayerManager.IsSpawned;
                if (!_current.multiplayerManagerSpawned) Errors.Add("Shared multiplayer manager did not spawn.");
                var startButton = UnityEngine.Object.FindObjectsByType<HoverableStartButton>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault();
                _current.hoverStartButton = startButton != null;
                if (startButton != null)
                {
                    // The Star mode Trigger button must start the game without hand tracking.
                    HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
                    yield return EditorAppDriver.RenderMainCameraFor(WarmupSeconds);
                    _current.startedWithoutHand = startButton == null || !startButton.isActiveAndEnabled;
                    if (!_current.startedWithoutHand) Errors.Add("Trigger button did not start the game without a hand.");
                }
                AuditLoadedScenes();
                yield return new WaitForEndOfFrame();
                RenderMainCamera();
                Save();

                if (NetworkManager.Singleton != null) app.Shutdown();
                else app.ExitNoLiDARScene();
                for (var t = 0f; SceneManager.GetActiveScene().name != gallery && t < SceneLoadTimeout; t += Time.unscaledDeltaTime)
                    yield return null;
                yield return new WaitForSecondsRealtime(1);
                _current.returnedToGallery = SceneManager.GetActiveScene().name == gallery;
                if (!_current.returnedToGallery) Errors.Add("Exit did not return to the gallery scene.");
                Debug.Log($"[PLAYTHROUGH] EXIT {_current.scene} role={role} errors={_current.errors.Count}");
                _current = null;
                Save();
                yield return RestartXR();
            }
        }

        // XR Simulation keeps its simulated room in a scene that the app's single-mode scene loads unload, and
        // does not rebuild it for the next AR session, so later cases rendered black. Restarting XR at the gallery
        // (which has no AR session) gives every case the room. Editor-only; devices are unaffected.
        private static IEnumerator RestartXR()
        {
            var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
            if (manager == null || manager.activeLoader == null) yield break;
            manager.StopSubsystems();
            manager.DeinitializeLoader();
            yield return manager.InitializeLoader();
            manager.StartSubsystems();
        }

        private void AuditLoadedScenes()
        {
            var roots = new List<GameObject>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
                roots.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            foreach (var root in roots)
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                if (missing == 0) continue;
                _current.missingScripts += missing;
                Errors.Add($"Missing script x{missing} on {PathOf(transform)}");
            }

            var renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _current.renderers = renderers.Length;
            foreach (var renderer in renderers)
            {
                if (!renderer.enabled) continue;
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        // Unassigned slots render nothing; worth a look but not always a defect.
                        _current.warnings.Add($"Null material slot on {PathOf(renderer.transform)}");
                        continue;
                    }
                    var shader = material.shader;
                    if (shader == null || shader.name == "Hidden/InternalErrorShader" || !shader.isSupported)
                        Errors.Add($"Unsupported shader '{shader?.name}' in material '{material.name}' on {PathOf(renderer.transform)}");
                }
            }

            foreach (var vfx in UnityEngine.Object.FindObjectsByType<VisualEffect>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (vfx.enabled && vfx.visualEffectAsset == null)
                    Errors.Add("VisualEffect without asset on " + PathOf(vfx.transform));
        }

        private void RenderMainCamera()
        {
            var camera = Camera.main;
            if (camera == null)
                camera = Camera.allCameras.Where(c => c.targetTexture == null).OrderByDescending(c => c.depth).FirstOrDefault();
            if (camera == null) { Errors.Add("No enabled camera to render."); return; }
            _current.camera = PathOf(camera.transform);
            var background = camera.GetComponent<ARCameraBackground>();
            _current.cameraBackground = background != null && background.enabled;

            var target = RenderTexture.GetTemporary(RenderWidth, RenderHeight, 24, RenderTextureFormat.ARGB32);
            var request = new RenderPipeline.StandardRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request))
                RenderPipeline.SubmitRenderRequest(camera, request);
            else
            {
                var previous = camera.targetTexture;
                camera.targetTexture = target;
                camera.Render();
                camera.targetTexture = previous;
            }

            var active = RenderTexture.active;
            RenderTexture.active = target;
            var texture = new Texture2D(RenderWidth, RenderHeight, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, RenderWidth, RenderHeight), 0, 0);
            texture.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(target);

            var pixels = texture.GetPixels32();
            // Unity draws broken shaders in saturated magenta.
            var magenta = pixels.Count(p => p.r > 230 && p.b > 230 && p.g < 40);
            _current.magentaFraction = (float)magenta / pixels.Length;
            if (_current.magentaFraction > 0.002f)
                Errors.Add($"Render is {_current.magentaFraction:P1} error-shader magenta.");
            // XR Simulation's room is always in view, so a frame that is almost all black drew nothing.
            _current.litFraction = (float)pixels.Count(p => p.r + p.g + p.b > 30) / pixels.Length;
            if (_current.litFraction < 0.01f)
                Errors.Add($"Render is black ({_current.litFraction:P1} of pixels lit): nothing was drawn.");
            _current.render = $"{_current.scene}-{_current.role}.png";
            File.WriteAllBytes(Path.Combine(_directory, _current.render), texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
        }

        private List<string> Errors => _current == null ? _report.startupErrors : _current.errors;

        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                lock (_report)
                {
                    if (_current != null && stackTrace.Contains("UnityEngine.XR.Simulation."))
                        _current.simulationErrors.Add(message + "\n" + stackTrace);
                    else
                        Errors.Add(message + "\n" + stackTrace);
                }
            }
            else if (type == LogType.Warning && _current != null && message.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0)
                lock (_report) _current.warnings.Add(message);
        }

        private string ReportPath => Path.Combine(_directory, "results.json");
        private void Save() => File.WriteAllText(ReportPath, JsonUtility.ToJson(_report, true));
        private static string Head(string text) => text.Split('\n')[0];

        private static string PathOf(Transform transform)
        {
            var path = transform.name;
            for (var parent = transform.parent; parent != null; parent = parent.parent) path = parent.name + "/" + path;
            return transform.gameObject.scene.name + ":" + path;
        }
    }
}
