#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace HoloKit.DeviceTests
{
    /// <summary>Opt-in device smoke run. Does not simulate hand input or remote peers.</summary>
    public sealed class DeviceSceneSmokeTests : MonoBehaviour
    {
        [Serializable] private sealed class SceneResult
        {
            public string scene;
            public string role;
            public string arState;
            public bool hostListening;
            public bool multiplayerManagerSpawned;
            public bool returnedToGallery;
            public string screenshot;
            public List<string> errors = new();
        }

        [Serializable] private sealed class Report
        {
            public string unityVersion;
            public string device;
            public string operatingSystem;
            public string product;
            public bool complete;
            public List<string> startupErrors = new();
            public List<SceneResult> scenes = new();
        }

        private readonly Report _report = new();
        private SceneResult _current;
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            if (!Environment.GetCommandLineArgs().Contains("--holokit-scene-test") &&
                Environment.GetEnvironmentVariable("HOLOKIT_SCENE_TEST") != "1") return;
            Debug.Log("[DEVICE TEST] Starting requested device scene smoke tests.");
            var runner = new GameObject("HoloKit Device Scene Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceSceneSmokeTests>();
        }

        private IEnumerator Start()
        {
            Debug.unityLogger.logEnabled = true;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "HoloKitSceneTests");
            Directory.CreateDirectory(_directory);
            _report.unityVersion = Application.unityVersion;
            _report.device = SystemInfo.deviceModel;
            _report.operatingSystem = SystemInfo.operatingSystem;
            _report.product = Application.identifier;
            Save();
            var run = Run();
            while (true)
            {
                bool more;
                object next;
                try { more = run.MoveNext(); next = more ? run.Current : null; }
                catch (Exception exception)
                {
                    Errors.Add(exception.ToString());
                    break;
                }
                if (!more) { _report.complete = true; break; }
                yield return next;
            }
            Save();
            Application.logMessageReceived -= OnLog;
            var failures = _report.startupErrors.Count + _report.scenes.Sum(s => s.errors.Count);
            Debug.Log($"[DEVICE TEST] COMPLETE={_report.complete} scenes={_report.scenes.Count} failures={failures} report={_directory}/results.json");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            if (app == null) throw new InvalidOperationException("App bootstrap did not create HoloKitApp.");
            // Native permissions remain system controlled; a device operator may need to respond.
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                throw new InvalidOperationException("Camera permission is required for device AR tests.");
            var scenes = app.GlobalSettings.GetAllRealities().ToArray();
            if (scenes.Length == 0) throw new InvalidOperationException("The product catalog is empty.");
            var filter = Environment.GetCommandLineArgs().FirstOrDefault(a => a.StartsWith("--holokit-test-scene="));
            var sceneName = filter == null ? Environment.GetEnvironmentVariable("HOLOKIT_TEST_SCENE") : filter.Substring("--holokit-test-scene=".Length);
            if (!string.IsNullOrEmpty(sceneName))
            {
                scenes = scenes.Where(r => r.Scene.SceneName == sceneName).ToArray();
                if (scenes.Length == 0) throw new InvalidOperationException("Requested scene is absent from the product catalog: " + sceneName);
            }
            foreach (var reality in scenes)
            foreach (var role in new[] { HoloKitAppPlayerType.Player, HoloKitAppPlayerType.Spectator })
            {
                _current = new SceneResult { scene = reality.Scene.SceneName, role = role.ToString() };
                _report.scenes.Add(_current);
                Save();
                Debug.Log($"[DEVICE TEST] ENTER {_current.scene} role={role}");
                app.CurrentReality = reality;
                app.EnterRealityAs(true, role);
                yield return new WaitForSecondsRealtime(6);
                if (SceneManager.GetActiveScene().name != _current.scene)
                    Errors.Add("Unexpected active scene: " + SceneManager.GetActiveScene().name);
                if (app.RealityManager == null) Errors.Add("Reality manager did not initialize.");
                _current.hostListening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsListening;
                if (!_current.hostListening) Errors.Add("Local network host did not start.");
                _current.multiplayerManagerSpawned = app.MultiplayerManager != null && app.MultiplayerManager.IsSpawned;
                if (!_current.multiplayerManagerSpawned) Errors.Add("Shared multiplayer manager did not spawn.");
                _current.arState = ARSession.state.ToString();
                if (ARSession.state != ARSessionState.SessionTracking)
                    Errors.Add("AR did not reach SessionTracking: " + _current.arState);
                // Capture frames for visual inspection; startup success does not prove correct rendering.
                yield return new WaitForEndOfFrame();
                _current.screenshot = _current.scene + "-" + role + ".png";
                var texture = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Path.Combine(_directory, _current.screenshot), texture.EncodeToPNG());
                Destroy(texture);
                Save();
                if (NetworkManager.Singleton != null) app.Shutdown();
                else app.ExitNoLiDARScene();
                yield return new WaitForSecondsRealtime(3);
                _current.returnedToGallery = SceneManager.GetActiveScene().name == "Start";
                if (!_current.returnedToGallery) Errors.Add("Exit did not return to the gallery scene.");
                Save();
                Debug.Log($"[DEVICE TEST] EXIT {_current.scene} role={role} errors={_current.errors.Count}");
            }
        }

        private List<string> Errors => _current == null ? _report.startupErrors : _current.errors;
        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                Errors.Add(message + "\n" + stackTrace);
        }
        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
