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
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.Typed.TheFingerRibbon
{
    // Opt-in physical-device check that the five ribbons appear with the host's hand, sit on the
    // fingertips and trail them as the hand moves. The hand pose is injected after recognition.
    public sealed class DeviceFingerRibbonInteractionTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public bool complete;
            public bool arTracking;
            public bool ribbonsHiddenBeforeHand;
            public bool handValid;
            public bool ribbonsShownWithHand;
            public int ribbonCount;
            public float farthestRibbonFromItsTip;
            public float leastRibbonMovement;
            public List<int> aliveParticlesAfterMove = new();
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private const float FingerLength = .16f;
        private const float MoveDistance = .4f;
        private static readonly HandJoint[] Tips =
            { HandJoint.ThumbTip, HandJoint.IndexTip, HandJoint.MiddleTip, HandJoint.RingTip, HandJoint.LittleTip };

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
            Debug.Log($"[FINGER RIBBON INTERACTION TEST] complete={_report.complete} shown={_report.ribbonsShownWithHand} farthest={_report.farthestRibbonFromItsTip:F3} moved={_report.leastRibbonMovement:F3} errors={_report.errors.Count}");
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
            var manager = FindObjectsByType<FingerTextManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
            Require(tracker != null && manager != null, "Hand tracker or finger ribbon manager missing.");
            var ribbons = manager.GetComponentsInChildren<VisualEffect>(true);
            _report.ribbonCount = ribbons.Length;
            Require(ribbons.Length == Tips.Length, $"Expected {Tips.Length} finger ribbons, found {ribbons.Length}.");
            _report.ribbonsHiddenBeforeHand = !manager.gameObject.activeInHierarchy;
            Require(_report.ribbonsHiddenBeforeHand, "Ribbons were shown before any hand was seen.");

            // An open hand in front of the camera, fingers fanned in the view plane.
            var eye = HoloKitCameraManager.Instance.CenterEyePose;
            var wrist = eye.position + eye.forward * .5f;
            var directions = Enumerable.Range(0, Tips.Length)
                .Select(f => Quaternion.AngleAxis(-60f + 30f * f, eye.forward) * eye.up).ToArray();
            var joints = Hand(wrist, directions);
            for (var i = 0; i < 90; i++) { tracker.InjectPoseForDeviceTest(joints); yield return null; }
            _report.handValid = tracker.IsValid;
            _report.ribbonsShownWithHand = manager.gameObject.activeInHierarchy;
            Require(_report.handValid && _report.ribbonsShownWithHand, "Ribbons did not appear when the hand was recognized.");
            // Each ribbon rides on its own fingertip.
            var tips = Tips.Select(t => joints[(int)t]).ToArray();
            _report.farthestRibbonFromItsTip = ribbons.Max(r => tips.Min(t => Vector3.Distance(r.transform.position, t)));
            var claimed = ribbons.Select(r => Array.IndexOf(tips, tips.OrderBy(t => Vector3.Distance(r.transform.position, t)).First())).Distinct().Count();
            Save();
            Require(_report.farthestRibbonFromItsTip < .03f && claimed == Tips.Length,
                $"Ribbons are not on the fingertips (farthest {_report.farthestRibbonFromItsTip:F3} m, {claimed} tips used).");
            Capture("hand.png");

            // Sweep the hand sideways: every ribbon follows and leaves a trail.
            var before = ribbons.Select(r => r.transform.position).ToArray();
            for (var i = 1; i <= 60; i++)
            {
                tracker.InjectPoseForDeviceTest(Hand(wrist + eye.right * (MoveDistance * i / 60f), directions));
                yield return null;
            }
            joints = Hand(wrist + eye.right * MoveDistance, directions);
            for (var i = 0; i < 60; i++) { tracker.InjectPoseForDeviceTest(joints); yield return null; }
            _report.leastRibbonMovement = ribbons.Select((r, i) => Vector3.Distance(r.transform.position, before[i])).Min();
            _report.aliveParticlesAfterMove = ribbons.Select(r => r.aliveParticleCount).ToList();
            Save();
            Require(_report.leastRibbonMovement > MoveDistance * .75f, $"A ribbon did not follow the hand (moved {_report.leastRibbonMovement:F3} m).");
            Require(_report.aliveParticlesAfterMove.All(n => n > 0), "A ribbon left no trail while the hand moved.");
            Capture("trail.png");
        }

        /// <summary>A 21-joint hand: each finger's four joints lie along its direction from the wrist.</summary>
        private static Vector3[] Hand(Vector3 wrist, Vector3[] directions)
        {
            var joints = new Vector3[21];
            joints[(int)HandJoint.Wrist] = wrist;
            for (var finger = 0; finger < Tips.Length; finger++)
            for (var segment = 1; segment <= 4; segment++)
                joints[(int)Tips[finger] - 4 + segment] = wrist + directions[finger] * (FingerLength * segment / 4f);
            return joints;
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
