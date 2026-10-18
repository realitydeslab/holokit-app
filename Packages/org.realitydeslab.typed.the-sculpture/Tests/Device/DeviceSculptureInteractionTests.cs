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
            public string sculptureObject;
            public Bounds sculptureBounds;
            public HoloKit.DeviceTests.DeviceInspection.Result sculptureInspection;
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
            var pose = Hand(eye.position + eye.forward * .6f, eye.forward);
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
            var spawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var existing = spawned.Keys.ToList();
            var before = spawned.Count;
            for (var i = 0; i < 100 && spawned.Count == before; i++)
            {
                hover.OnLoaded(.1f);
                yield return new WaitForSecondsRealtime(.02f);
            }
            _report.sculptureSpawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count > before;
            Require(_report.sculptureSpawned, "Hover dwell did not spawn sculpture.");
            Capture("sculpture-started.png");

            // The phone may not face the sculpture, so render it from a virtual camera to see that it draws.
            yield return new WaitForSecondsRealtime(3);
            var sculpture = spawned.Where(o => !existing.Contains(o.Key)).Select(o => o.Value)
                .OrderByDescending(o => o.GetComponentsInChildren<Renderer>().Length).First();
            _report.sculptureObject = sculpture.name;
            var renderers = sculpture.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
            Require(renderers.Length > 0, $"The spawned {sculpture.name} has no enabled renderers.");
            var bounds = renderers.Select(r => r.bounds).Aggregate((a, b) => { a.Encapsulate(b); return a; });
            _report.sculptureBounds = bounds;
            // The figure is a VFX Graph effect that only simulates while seen; the phone may not have
            // faced it when it spawned, so restart it once the inspection camera looks at it.
            var effects = sculpture.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>();
            // The figure fills with letters launched from the host's hand: its spawner runs only while a hand is tracked
            // (IsInteractable), and the tracker drops a hand silent for 0.33 s. Keep a hand in the figure, as a host
            // reaching into it would, while the inspection camera looks.
            var holding = true;
            IEnumerator HoldHand()
            {
                var inFigure = Hand(bounds.center, bounds.center - Camera.main.transform.position);
                while (holding)
                {
                    tracker.InjectPoseForDeviceTest(inFigure);
                    yield return null;
                }
            }
            // The letters face the player's view direction (CameraForwardVfxFeeder), and the phone may lie flat looking
            // down; turn them to the inspection camera while it looks.
            var feeders = sculpture.GetComponentsInChildren<CameraForwardVfxFeeder>().Where(f => f.enabled).ToList();
            StartCoroutine(HoldHand());
            // Letters take seconds to fly from the hand into the figure, which fills far less than the effect's bounds.
            yield return HoloKit.DeviceTests.DeviceInspection.Capture(bounds.center, Mathf.Max(1.5f, bounds.extents.y * 1.7f),
                effects.Cast<Component>().Concat(renderers.Where(r => r.GetComponent<UnityEngine.VFX.VisualEffect>() == null)),
                _directory, "sculpture-inspection", result => _report.sculptureInspection = result,
                warmupSeconds: 6f, onCameraReady: camera =>
                {
                    feeders.ForEach(f => f.enabled = false);
                    foreach (var effect in effects)
                    {
                        effect.SetVector3("CameraForward", camera.transform.forward);
                        effect.Reinit();
                    }
                },
                probe: () => string.Join(", ", effects.Select(e => $"{e.name}: {e.aliveParticleCount} particles, culled={e.culled}, interactable={e.GetBool("IsInteractable")}")));
            holding = false;
            feeders.ForEach(f => f.enabled = true);
            Save();
            Require(_report.sculptureInspection.objectPixels > .01f,
                $"The sculpture draws {_report.sculptureInspection.objectPixels:P1} of a close-up view.");
        }

        // A hand as the tracker reports one: 21 distinct joints spread over the palm, fingers along toward. With every
        // joint at one point (no real hand) the sculpture's letters never form, so its close-up stayed empty.
        private static Vector3[] Hand(Vector3 palm, Vector3 toward)
        {
            toward = toward.normalized;
            var side = Vector3.Cross(Vector3.up, toward);
            side = side.sqrMagnitude > 1e-4f ? side.normalized : Vector3.right;
            var joints = new Vector3[21];
            for (var i = 0; i < 21; i++)
                joints[i] = palm + toward * (0.012f * (i % 5)) + side * (0.02f * (i / 5 - 2));
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
