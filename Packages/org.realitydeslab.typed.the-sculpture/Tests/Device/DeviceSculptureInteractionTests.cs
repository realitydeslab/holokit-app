#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.Typed.TheSculpture
{
    // Opt-in device check of tracked hand, hover dwell and sculpture spawn.
    public sealed class DeviceSculptureInteractionTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public bool complete;
            public bool arTracking;
            public bool handValid;
            public bool handHoverEnabled;
            public bool syntheticPlacement;
            public bool sculptureSpawned;
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (!Environment.GetCommandLineArgs().Contains("--sculpture-interaction-test") &&
                Environment.GetEnvironmentVariable("SCULPTURE_INTERACTION_TEST") != "1") return;
            var runner = new GameObject("Sculpture Device Interaction Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceSculptureInteractionTests>();
        }

        private IEnumerator Start()
        {
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "SculptureInteractionTests");
            Directory.CreateDirectory(_directory);
            Save();
            var run = Run();
            while (true)
            {
                bool more;
                object next;
                try { more = run.MoveNext(); next = more ? run.Current : null; }
                catch (Exception e) { _report.errors.Add(e.ToString()); break; }
                if (!more) { _report.complete = true; break; }
                yield return next;
            }
            Save();
            Application.logMessageReceived -= OnLog;
            Debug.Log($"[SCULPTURE INTERACTION TEST] complete={_report.complete} hand={_report.handHoverEnabled} sculpture={_report.sculptureSpawned} errors={_report.errors.Count}");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp missing.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission missing.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "TypedTheSculpture");
            Require(reality != null, "Sculpture missing from catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "TypedTheSculpture", "Sculpture scene did not open.");
            for (var i = 0; i < 300 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            Require(_report.arTracking, "AR tracking unavailable: " + ARSession.state);
            var manager = app.RealityManager as TypedTheSculptureRealityManager;
            var tracker = HoloKitHandTracker.Instance;
            Require(manager != null && tracker != null, "Reality manager or hand tracker missing.");
            var eye = HoloKitCameraManager.Instance.CenterEyePose;
            var pose = new Vector3[21];
            for (var i = 0; i < 21; i++) pose[i] = eye.position + eye.forward * .6f;
            for (var i = 0; i < 25; i++) { tracker.InjectPoseForDeviceTest(pose); yield return null; }
            _report.handValid = tracker.IsValid;
            _report.handHoverEnabled = manager.HostHandPose.IsActive;
            Require(_report.handValid && _report.handHoverEnabled, "Tracked hand did not enable hover interaction.");

            var placement = FindObjectOfType<ARPlacementManager>(true);
            var startButton = FindObjectOfType<HoverableStartButton>(true);
            Require(placement != null && startButton != null, "Placement or hover start missing.");
            for (var i = 0; i < 60 && !placement.IsValid; i++) yield return new WaitForSecondsRealtime(.1f);
            if (!placement.IsValid)
            {
                placement.IsActive = false;
                placement.HitPoint.position = eye.position + eye.forward * 1.5f;
                placement.HitPoint.gameObject.SetActive(true);
                placement.OnFoundPlane?.Invoke();
                _report.syntheticPlacement = true;
            }
            yield return new WaitForSecondsRealtime(1);
            var hover = startButton.GetComponent<HoverableObject>();
            Require(hover != null && startButton.gameObject.activeInHierarchy, "Hover start control is not visible.");
            var before = NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count;
            for (var i = 0; i < 100 && NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count == before; i++)
            {
                hover.OnLoaded(.1f);
                yield return new WaitForSecondsRealtime(.02f);
            }
            _report.sculptureSpawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count > before;
            Require(_report.sculptureSpawned, "Hover dwell did not spawn sculpture.");
            Capture("sculpture-started.png");
        }

        private void Capture(string name)
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(_directory, name), texture.EncodeToPNG());
            Destroy(texture);
            _report.screenshots.Add(name);
            Save();
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private void OnLog(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                _report.errors.Add(message + "\n" + stack);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
