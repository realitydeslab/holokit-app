#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.HoloKitAppLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.Typed.TheFingerRibbon
{
    // Opt-in physical-device check of finger curl and hand shake response.
    public sealed class DeviceFingerRibbonInteractionTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public bool complete;
            public bool arTracking;
            public bool handValid;
            public bool palmDetected;
            public bool fistDetected;
            public bool shakeMovedRibbon;
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (!Environment.GetCommandLineArgs().Contains("--finger-ribbon-interaction-test") &&
                Environment.GetEnvironmentVariable("FINGER_RIBBON_INTERACTION_TEST") != "1") return;
            var runner = new GameObject("Finger Ribbon Device Interaction Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceFingerRibbonInteractionTests>();
        }

        private IEnumerator Start()
        {
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "FingerRibbonInteractionTests");
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
            Debug.Log($"[FINGER RIBBON INTERACTION TEST] complete={_report.complete} palm={_report.palmDetected} fist={_report.fistDetected} shake={_report.shakeMovedRibbon} errors={_report.errors.Count}");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp missing.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission missing.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "TypedTheFingerRibbon");
            Require(reality != null, "Finger Ribbon missing from catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "TypedTheFingerRibbon", "Finger Ribbon scene did not open.");
            for (var i = 0; i < 300 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            Require(_report.arTracking, "AR tracking unavailable: " + ARSession.state);
            var tracker = HoloKitHandTracker.Instance;
            var gesture = FindObjectOfType<HandGestureManager>(true);
            Require(tracker != null && gesture != null, "Hand tracker or gesture manager missing.");
            var eye = HoloKitCameraManager.Instance.CenterEyePose;
            var origin = eye.position + eye.forward * .6f;
            var joints = new Vector3[21];
            for (var i = 0; i < 21; i++) joints[i] = origin;
            joints[(int)HandJoint.MiddleMCP] += Vector3.up * .05f;
            joints[(int)HandJoint.MiddlePIP] += Vector3.up * .1f;
            joints[(int)HandJoint.MiddleDIP] += Vector3.up * .15f;
            joints[(int)HandJoint.MiddleTip] += Vector3.up * .23f;
            for (var i = 0; i < 25; i++) { tracker.InjectPoseForDeviceTest(joints); yield return null; }
            _report.handValid = tracker.IsValid;
            _report.palmDetected = gesture.CurrentGesture == HandGestureManager.Gesture.Palm;
            Require(_report.handValid && _report.palmDetected, "Open palm gesture was not recognized.");
            Capture("palm.png");
            joints[(int)HandJoint.MiddleTip] = origin + Vector3.up * .02f;
            for (var i = 0; i < 25; i++) { tracker.InjectPoseForDeviceTest(joints); yield return null; }
            _report.fistDetected = gesture.CurrentGesture == HandGestureManager.Gesture.Fist;
            Require(_report.fistDetected, "Closed fist gesture was not recognized.");
            Capture("fist.png");
            var start = gesture.HandTips[2].position;
            for (var i = 0; i < 40; i++)
            {
                var delta = (i % 10 < 5 ? .15f : -.15f) * Vector3.right;
                for (var joint = 0; joint < 21; joint++) joints[joint] += delta;
                tracker.InjectPoseForDeviceTest(joints);
                yield return null;
            }
            _report.shakeMovedRibbon = Vector3.Distance(start, gesture.HandTips[2].position) > .05f ||
                                       gesture.TipVelocityDirection[2].sqrMagnitude > .1f;
            Require(_report.shakeMovedRibbon, "Shaking did not drive the finger ribbon tip.");
            Capture("shake.png");
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
