#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.QuantumRealm.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.QuantumRealm
{
    // Opt-in device check of the hand, hover-to-start and Buddha switch paths.
    public sealed class DeviceQuantumRealmInteractionTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public bool complete;
            public bool arTracking;
            public bool syntheticPlacement;
            public bool handValid;
            public bool handMoved;
            public bool hoverStartedSession;
            public bool buddhaSwitched;
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (!Environment.GetCommandLineArgs().Contains("--qrealm-interaction-test") &&
                Environment.GetEnvironmentVariable("QREALM_INTERACTION_TEST") != "1") return;
            var runner = new GameObject("QRealm Device Interaction Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceQuantumRealmInteractionTests>();
        }

        private IEnumerator Start()
        {
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "QRealmInteractionTests");
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
            Debug.Log($"[QREALM INTERACTION TEST] complete={_report.complete} hand={_report.handMoved} hover={_report.hoverStartedSession} switch={_report.buddhaSwitched} errors={_report.errors.Count}");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp missing.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission missing.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "QuantumRealm");
            Require(reality != null, "Quantum Realm missing from catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "QuantumRealm", "Quantum Realm scene did not open.");
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            Require(_report.arTracking, "AR tracking unavailable.");
            var manager = app.RealityManager as QuantumRealmRealityManager;
            var tracker = HoloKitHandTracker.Instance;
            Require(manager != null && tracker != null, "Reality manager or hand tracker missing.");
            var eye = HoloKitCameraManager.Instance.CenterEyePose;
            var pose = new Vector3[21];
            var basePosition = eye.position + eye.forward * 0.6f;
            for (var i = 0; i < 21; i++) pose[i] = basePosition;
            tracker.InjectPoseForDeviceTest(pose);
            yield return new WaitForSecondsRealtime(.35f);
            _report.handValid = tracker.IsValid;
            var first = manager.HostHandPoint.position;
            for (var i = 0; i < 21; i++) pose[i] += Vector3.right * 0.35f;
            for (var frame = 0; frame < 35; frame++)
            {
                tracker.InjectPoseForDeviceTest(pose);
                yield return null;
            }
            _report.handMoved = Vector3.Distance(first, manager.HostHandPoint.position) > .05f;
            Require(_report.handValid && _report.handMoved, "Synthetic hand motion did not move the host hand point.");
            Capture("hand-motion.png");

            var placement = FindObjectOfType<ARPlacementManager>(true);
            var startButton = FindObjectOfType<HoverableStartButton>(true);
            Require(placement != null && startButton != null, "Placement or hover-to-start button missing.");
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
            Require(hover != null && startButton.gameObject.activeInHierarchy, "Hover-to-start control is not visible.");
            // OnLoaded is the same dwell callback emitted by the hand HoverObject.
            for (var i = 0; i < 100 && FindObjectOfType<BuddhaGroup>(true) == null; i++)
            {
                hover.OnLoaded(.1f);
                yield return new WaitForSecondsRealtime(.02f);
            }
            yield return new WaitForSecondsRealtime(.5f);
            var group = FindObjectOfType<BuddhaGroup>(true);
            _report.hoverStartedSession = group != null && group.IsSpawned;
            Require(_report.hoverStartedSession, "Hand hover did not start the Buddha session.");
            Capture("buddha-started.png");
            var panel = FindObjectOfType<QuantumRealmUIPanel>(true);
            Require(panel != null, "Buddha switch panel missing.");
            var before = group.CurrentBuddhaIndexForDeviceTest;
            panel.OnSwitchButtonPressedFunc();
            yield return new WaitForSecondsRealtime(.5f);
            _report.buddhaSwitched = group.CurrentBuddhaIndexForDeviceTest != before;
            Require(_report.buddhaSwitched, "Buddha switch button did not change the active object.");
            Capture("buddha-switched.png");
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
