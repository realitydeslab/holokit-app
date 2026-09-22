#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.TheHunting.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheHunting
{
    // Opt-in iPhone check for the actual summon button and networked dragon spawn.
    public sealed class DeviceMofaHuntingSpawnTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public string device;
            public bool complete;
            public bool arTracking;
            public bool hostListening;
            public bool summonButtonActive;
            public bool defaultPlacementUsed;
            public bool dragonSpawned;
            public bool portalSpawned;
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            if (!Environment.GetCommandLineArgs().Contains("--mofa-hunting-spawn-test") &&
                Environment.GetEnvironmentVariable("MOFA_HUNTING_SPAWN_TEST") != "1") return;
            var runner = new GameObject("MOFA Hunting Device Spawn Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaHuntingSpawnTests>();
        }

        private IEnumerator Start()
        {
            var previousSleepTimeout = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "MofaHuntingSpawnTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            Save();
            var run = Run();
            while (true)
            {
                bool more;
                object next;
                try { more = run.MoveNext(); next = more ? run.Current : null; }
                catch (Exception exception) { _report.errors.Add(exception.ToString()); break; }
                if (!more) { _report.complete = true; break; }
                yield return next;
            }
            Save();
            Application.logMessageReceived -= OnLog;
            Screen.sleepTimeout = previousSleepTimeout;
            Debug.Log($"[MOFA HUNTING TEST] complete={_report.complete} dragon={_report.dragonSpawned} errors={_report.errors.Count}");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission is required.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheHunting");
            Require(reality != null, "Hunting is missing from the product catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "MOFATheHunting", "Hunting scene did not open.");
            var manager = app.RealityManager as MofaHuntingRealityManager;
            Require(manager != null, "Hunting manager did not initialize.");
            for (var i = 0; i < 300 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            _report.hostListening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsListening;
            Save();
            Require(_report.arTracking && _report.hostListening, "AR tracking or host networking is not ready.");
            var placement = FindObjectOfType<ARPlacementManager>(true);
            Require(placement != null && placement.IsActive, "Hunting placement manager is unavailable.");
            var panel = FindObjectsOfType<MofaHuntingDragonControllerUIPanel>(true)
                .FirstOrDefault(item => item.isActiveAndEnabled);
            _report.summonButtonActive = panel != null;
            Require(panel != null, "Dragon summon panel is not visible.");
            yield return new WaitForSecondsRealtime(1);
            // Exercise the failure reported on a phone without a classified floor.
            placement.HitPoint.gameObject.SetActive(false);
            panel.OnSpawnDragonButtonPressedFunc();
            for (var i = 0; i < 200 && manager.DragonController == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.defaultPlacementUsed = manager.UsedDefaultPlacement;
            _report.dragonSpawned = manager.DragonController != null;
            _report.portalSpawned = manager.PortalController != null;
            Save();
            yield return new WaitForEndOfFrame();
            var screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(_directory, "dragon-summoned.png"), screenshot.EncodeToPNG());
            Destroy(screenshot);
            Require(_report.defaultPlacementUsed && _report.dragonSpawned && _report.portalSpawned,
                "The summon button did not spawn the dragon and portal using default placement.");
            Require(_report.errors.Count == 0, "Hunting produced runtime errors: " + _report.errors.Count);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                _report.errors.Add(message + "\n" + stackTrace);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
