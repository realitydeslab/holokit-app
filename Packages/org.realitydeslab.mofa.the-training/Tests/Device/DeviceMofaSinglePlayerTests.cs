#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HoloKit;
using HoloKit.iOS;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.Library.Base.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheTraining
{
    /// <summary>
    /// Opt-in physical-iPhone check of MOFA single player (Training, Mono): the camera agrees with ARKit in portrait and
    /// landscape, watch messages delivered through the iPhone's WatchConnectivity session cast spells, and a screen
    /// recording made during the fight is written. Each part reports separately; the recording runs last because
    /// saving it to Photos may show a system prompt.
    /// </summary>
    public sealed class DeviceMofaSinglePlayerTests : MonoBehaviour
    {
        [Serializable]
        private sealed class OrientationCheck
        {
            public string orientation;
            public string interfaceOrientation; // UIKit's, which ARKit XR renders with
            public int width;
            public int height;
            public bool referenceAvailable;
            // Degrees between AR Foundation's camera and ARKit's camera for this interface orientation, as scripts see
            // it after Update.
            public float cameraAngle;
            // The same as a script reads it during Update, and as the screen renders it.
            public float scriptUpdateAngle;
            public float screenRenderAngle;
            public float screenBackgroundError;
            public int screenRenders;
            // Degrees between Mono's Center Eye Pose and the camera.
            public float centerEyeAngle;
            public float unityFovH, unityFovV, arkitFovH, arkitFovV;
            public string screenshot;
        }

        [Serializable]
        private sealed class RealWatchWindow
        {
            public float seconds;
            public int swingsReceived;
            public int groundChanges;
            public int casts;
            public int reachableSeconds;
            public string roundStartContextError;
            public List<float> swingTimes = new();
        }

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string os;
            public string product;
            public bool complete;
            public bool arTracking;
            public List<OrientationCheck> orientations = new();
            public bool watchPaired;
            public bool watchAppInstalled;
            public bool watchReachable;
            public int watchSessionActivationState = -1;
            public string roundStartContextError;
            public bool fighting;
            public int swings;
            public int swingCasts;
            public bool groundStateReceived;
            public bool ultimateCast;
            public RealWatchWindow realWatch;
            public bool recordingStarted;
            public bool recordMicrophone;
            public bool microphoneAuthorized;
            // Frames the recorder rendered, and how far the worst one was from ARKit's camera pose and AR background.
            // The same for frames rendered during Update, when the recorder used to render.
            public int updateTimeFramesChecked;
            public float updateTimeCameraAngle;
            public float updateTimeBackgroundError;
            public int recordedFramesChecked;
            public float recordedCameraAngle;
            public float recordedBackgroundError;
            public int recordingAppendFailures;
            public string video;
            public long videoBytes;
            public bool videoFinished;
            // Screenshots taken while recording, and their time in the movie, to compare with its frames.
            public List<string> recordingShots = new();
            public List<float> recordingShotSeconds = new();
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private const string AppendFailure = "Cannot Append Video Frame";
        private int _updateTimeRenders;
        private bool _renderingInUpdate;
        private RenderTexture _probeBuffer, _probeFrame;
        private OrientationCheck _measuring;
        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (Environment.GetEnvironmentVariable("MOFA_SINGLE_TEST") != "1") return;
            var runner = new GameObject("MOFA Single Player Device Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaSinglePlayerTests>();
        }

        private IEnumerator Start()
        {
            var sleep = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "MofaSinglePlayerTests");
            if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.os = SystemInfo.operatingSystem;
            _report.product = Application.identifier;
            Save();
            yield return Section("setup", EnterTraining());
            if (_report.errors.Count == 0)
            {
                yield return Section("portrait", CheckOrientation(ScreenOrientation.Portrait));
                yield return Section("landscape-left", CheckOrientation(ScreenOrientation.LandscapeLeft));
                yield return Section("portrait-again", RestorePortrait());
                yield return Section("watch", Watch());
                if (Environment.GetEnvironmentVariable("MOFA_SINGLE_REAL_WATCH") == "1")
                    yield return Section("real-watch", RealWatch());
                yield return Section("recording", Recording());
            }
            _report.complete = true;
            Save();
            Application.logMessageReceived -= OnLog;
            Screen.sleepTimeout = sleep;
            Debug.Log($"[MOFA SINGLE TEST] complete errors={_report.errors.Count} report={_directory}/results.json");
            Destroy(gameObject);
        }

        // Runs one part; an exception fails that part and the run moves on.
        private IEnumerator Section(string name, IEnumerator run)
        {
            Debug.Log($"[MOFA SINGLE TEST] {name}");
            while (true)
            {
                object next;
                try
                {
                    if (!run.MoveNext()) break;
                    next = run.Current;
                }
                catch (Exception exception)
                {
                    _report.errors.Add($"{name}: {exception.Message}");
                    break;
                }
                yield return next;
            }
            Save();
        }

        private IEnumerator EnterTraining()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission is required.");
            var training = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheTraining");
            Require(training != null, "MOFA Training is missing from the product catalog.");
            app.CurrentReality = training;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "MOFATheTraining", "Training scene did not open.");
            var manager = Manager;
            Require(manager != null, "Training reality manager did not initialize.");
            for (var i = 0; i < 80 && manager.LocalMofaPlayer == null; i++) yield return new WaitForSecondsRealtime(.1f);
            Require(manager.LocalMofaPlayer != null, "Local MOFA player did not spawn.");
            for (var i = 0; i < 600 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            Require(_report.arTracking, "AR tracking unavailable: " + ARSession.state);
        }

        private static MofaTrainingRealityManager Manager => HoloKitApp.Instance.RealityManager as MofaTrainingRealityManager;

        private static Camera ARCamera => FindObjectOfType<ARCameraManager>()?.GetComponent<Camera>();

        private IEnumerator CheckOrientation(ScreenOrientation orientation)
        {
            var landscape = orientation is ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight;
            HoloKitCameraManager.Instance.ForceScreenOrientation = !landscape;
            Screen.orientation = orientation;
            for (var i = 0; i < 50 && (Screen.width > Screen.height) != landscape; i++)
                yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(1.5f);
            var camera = ARCamera;
            Require(camera != null, "AR camera missing.");
            var check = new OrientationCheck
            {
                orientation = Screen.orientation.ToString(),
                interfaceOrientation = ARKitCameraReference.InterfaceOrientation.ToString(),
                width = Screen.width,
                height = Screen.height
            };
            _report.orientations.Add(check);
            Require((Screen.width > Screen.height) == landscape, $"The screen did not turn {orientation}: {Screen.width}x{Screen.height}.");

            var centerEye = (Transform)typeof(HoloKitCameraManager)
                .GetField("_centerEyePose", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(HoloKitCameraManager.Instance);
            var background = camera.GetComponent<ARCameraBackground>();
            void OnScreenRender(ScriptableRenderContext context, Camera rendering)
            {
                if (rendering != camera || rendering.targetTexture != null) return;
                if (!ARKitCameraReference.TryGetReference(camera, out var reference, out _, out var display)) return;
                check.screenRenders++;
                check.screenRenderAngle = Mathf.Max(check.screenRenderAngle, Quaternion.Angle(camera.transform.localRotation, reference));
                var used = background.material.GetMatrix("_UnityDisplayTransform");
                for (var i = 0; i < 16; i++) check.screenBackgroundError = Mathf.Max(check.screenBackgroundError, Mathf.Abs(used[i] - display[i]));
            }
            RenderPipelineManager.beginCameraRendering += OnScreenRender;
            _measuring = check;
            var samples = 0;
            for (var frame = 0; frame < 30; frame++)
            {
                // A coroutine resumes after every script's Update: what constraints and LateUpdate scripts see.
                yield return null;
                if (!ARKitCameraReference.TryGetReference(camera, out var rotation, out var projection)) continue;
                samples++;
                check.cameraAngle = Mathf.Max(check.cameraAngle, Quaternion.Angle(camera.transform.localRotation, rotation));
                check.centerEyeAngle = Mathf.Max(check.centerEyeAngle, Quaternion.Angle(centerEye.rotation, camera.transform.rotation));
                check.unityFovH = Fov(camera.projectionMatrix.m00);
                check.unityFovV = Fov(camera.projectionMatrix.m11);
                check.arkitFovH = Fov(projection.m00);
                check.arkitFovV = Fov(projection.m11);
            }
            _measuring = null;
            RenderPipelineManager.beginCameraRendering -= OnScreenRender;
            check.referenceAvailable = samples > 0;
            yield return new WaitForEndOfFrame();
            check.screenshot = $"{orientation}.png";
            Capture(check.screenshot);

            Save();
            Require(check.referenceAvailable, "ARKit gave no camera reference.");
            Require(check.cameraAngle < 2f, $"The camera is {check.cameraAngle:F1} deg from ARKit's camera in {orientation}.");
            Require(check.scriptUpdateAngle < 2f, $"Scripts reading the camera in Update see it {check.scriptUpdateAngle:F1} deg from ARKit's camera in {orientation}.");
            Require(check.screenRenders > 0 && check.screenRenderAngle < 2f && check.screenBackgroundError < .05f,
                $"The screen renders the camera {check.screenRenderAngle:F1} deg from ARKit's, background off by {check.screenBackgroundError:F2} in {orientation}.");
            Require(check.centerEyeAngle < 2f, $"Mono's Center Eye Pose is {check.centerEyeAngle:F1} deg from the camera in {orientation}.");
            Require(Mathf.Abs(check.unityFovH - check.arkitFovH) < .5f && Mathf.Abs(check.unityFovV - check.arkitFovV) < .5f,
                $"Field of view {check.unityFovH:F1}x{check.unityFovV:F1} deg differs from ARKit's {check.arkitFovH:F1}x{check.arkitFovV:F1} in {orientation}.");
        }

        private static float Fov(float focal) => 2 * Mathf.Atan(1 / focal) * Mathf.Rad2Deg;

        private IEnumerator RestorePortrait()
        {
            HoloKitCameraManager.Instance.ForceScreenOrientation = true;
            Screen.orientation = ScreenOrientation.Portrait;
            for (var i = 0; i < 50 && Screen.width > Screen.height; i++) yield return new WaitForSecondsRealtime(.1f);
            Require(Screen.width < Screen.height, "The screen did not return to portrait.");
        }

        private IEnumerator Watch()
        {
            _report.watchPaired = HoloKitAppWatchConnectivityAPI.DeviceHasPairedAppleWatch();
            _report.watchAppInstalled = HoloKitAppWatchConnectivityAPI.IsWatchAppInstalled();
            _report.watchReachable = HoloKitAppWatchConnectivityAPI.IsWatchReachable();
            _report.watchSessionActivationState = MofaWatchConnectivityAPI.SessionActivationStateForDeviceTest;
            Save();

            var manager = Manager;
            var placement = FindObjectOfType<ARPlacementManager>(true);
            Require(placement != null, "Training placement manager is missing.");
            for (var i = 0; i < 80 && !placement.IsValid; i++) yield return new WaitForSecondsRealtime(.1f);
            if (!placement.IsValid)
            {
                placement.IsActive = false;
                placement.HitPoint.gameObject.SetActive(true);
            }
            var start = FindObjectsOfType<MofaUIPanel>(true).FirstOrDefault(panel => panel.isActiveAndEnabled);
            Require(start != null, "The visible MOFA start panel is missing.");
            start.OnTriggerButtonPressed();
            for (var i = 0; i < 120 && manager.CurrentPhase.Value != MofaPhase.Fighting; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.fighting = manager.CurrentPhase.Value == MofaPhase.Fighting;
            // Sent at Countdown: the watch starts sensing swings when this context reaches it.
            _report.roundStartContextError = MofaWatchConnectivityAPI.LastContextErrorForDeviceTest;
            Save();
            Require(_report.fighting, "Training did not reach Fighting: " + manager.CurrentPhase.Value);

            var school = HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId;
            var spells = manager.SpellList.List.Where(spell => spell.MagicSchool.TokenId.Equals(school)).ToList();
            var basic = spells.First(spell => spell.SpellType == SpellType.Basic);
            var ultimate = spells.First(spell => spell.SpellType == SpellType.Secondary);
            var player = manager.LocalMofaPlayer;
            yield return new WaitForSecondsRealtime(basic.ChargeTime * basic.MaxChargeCount + .5f);
            for (var i = 0; i < 3; i++)
            {
                var before = player.CastCount.Value;
                MofaWatchConnectivityAPI.ReceiveWatchTriggerForDeviceTest();
                _report.swings++;
                for (var t = 0; t < 20 && player.CastCount.Value <= before; t++) yield return new WaitForSecondsRealtime(.05f);
                if (player.CastCount.Value > before) _report.swingCasts++;
                if (i == 0)
                {
                    yield return new WaitForSecondsRealtime(.2f);
                    yield return new WaitForEndOfFrame();
                    Capture("watch-swing.png");
                }
                Save();
                yield return new WaitForSecondsRealtime(basic.ChargeTime + .2f);
            }
            Require(_report.swingCasts == _report.swings,
                $"{_report.swingCasts} of {_report.swings} watch swings cast a spell (shield intact: {ShieldIntact(player)}).");

            // Ground pose: the watch reports it, the ultimate charges, and the next swing casts it.
            var input = FindObjectOfType<MofaInputManager>(true);
            MofaWatchConnectivityAPI.ReceiveWatchStateForDeviceTest(MofaWatchState.Ground);
            for (var t = 0; t < 20 && input.CurrentWatchState != MofaWatchState.Ground; t++) yield return new WaitForSecondsRealtime(.05f);
            _report.groundStateReceived = input.CurrentWatchState == MofaWatchState.Ground;
            for (var t = 0; t < 300 && !(ShieldIntact(player) && input.SecondarySpellChargePercentage >= 1f); t++)
                yield return new WaitForSecondsRealtime(.05f);
            yield return new WaitForFixedUpdate(); // MofaInputManager re-checks the shield in FixedUpdate
            var seen = new HashSet<int>(SpawnedUltimates().Select(spell => spell.GetInstanceID()));
            MofaWatchConnectivityAPI.ReceiveWatchTriggerForDeviceTest();
            for (var t = 0; t < 30 && !_report.ultimateCast; t++)
            {
                yield return new WaitForSecondsRealtime(.05f);
                _report.ultimateCast = SpawnedUltimates().Any(spell => !seen.Contains(spell.GetInstanceID()));
            }
            yield return new WaitForSecondsRealtime(.3f);
            yield return new WaitForEndOfFrame();
            Capture("watch-ultimate.png");
            MofaWatchConnectivityAPI.ReceiveWatchStateForDeviceTest(MofaWatchState.Normal);
            Save();
            Require(_report.groundStateReceived, "The watch's Ground state did not reach MofaInputManager.");
            Require(_report.ultimateCast, $"A swing in the Ground pose did not cast the ultimate (shield intact: {ShieldIntact(player)}).");
        }

        // A person wearing the paired watch swings during this window; nothing is injected.
        private IEnumerator RealWatch()
        {
            const float Seconds = 90f;
            var manager = Manager;
            var player = manager.LocalMofaPlayer;
            if (manager.CurrentPhase.Value != MofaPhase.Fighting)
            {
                for (var i = 0; i < 300 && manager.CurrentPhase.Value != MofaPhase.Waiting; i++) yield return new WaitForSecondsRealtime(.1f);
                FindObjectsOfType<MofaUIPanel>(true).First(panel => panel.isActiveAndEnabled).OnTriggerButtonPressed();
                for (var i = 0; i < 120 && manager.CurrentPhase.Value != MofaPhase.Fighting; i++) yield return new WaitForSecondsRealtime(.1f);
            }
            Require(manager.CurrentPhase.Value == MofaPhase.Fighting, "Training did not reach Fighting for the real watch.");
            var window = _report.realWatch = new RealWatchWindow
            {
                roundStartContextError = MofaWatchConnectivityAPI.LastContextErrorForDeviceTest
            };
            var started = Time.realtimeSinceStartup;
            void OnSwing() { window.swingsReceived++; window.swingTimes.Add(Time.realtimeSinceStartup - started); }
            void OnState(MofaWatchState _) => window.groundChanges++;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnSwing;
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnState;
            var before = player.CastCount.Value;
            Debug.Log("[MOFA SINGLE TEST] Swing the watch now.");
            try
            {
                while (Time.realtimeSinceStartup - started < Seconds && window.swingsReceived < 5 &&
                       manager.CurrentPhase.Value == MofaPhase.Fighting)
                {
                    if (HoloKitAppWatchConnectivityAPI.IsWatchReachable()) window.reachableSeconds++;
                    window.seconds = Time.realtimeSinceStartup - started;
                    window.casts = player.CastCount.Value - before;
                    Save();
                    yield return new WaitForSecondsRealtime(1);
                }
            }
            finally
            {
                MofaWatchConnectivityAPI.OnWatchTriggered -= OnSwing;
                MofaWatchConnectivityAPI.OnWatchStateChanged -= OnState;
            }
            window.casts = player.CastCount.Value - before;
            Require(window.swingsReceived > 0, $"No swing arrived from the watch in {window.seconds:F0} s " +
                $"(reachable {window.reachableSeconds} s, round start context error: '{window.roundStartContextError}').");
        }

        private static bool ShieldIntact(MofaPlayer player) => player.LifeShield != null && !player.LifeShield.IsDestroyed;

        private static IEnumerable<Spell> SpawnedUltimates() =>
            FindObjectsOfType<Spell>().Where(spell => spell.SpellType == SpellType.Secondary &&
                spell.TryGetComponent<NetworkObject>(out var no) && no.IsSpawned);

        private IEnumerator Recording()
        {
            var recorder = FindObjectOfType<HoloKitVideoRecorder>(true);
            Require(recorder != null, "HoloKitVideoRecorder is missing.");
            var earlier = new HashSet<string>(Directory.GetFiles(Application.temporaryCachePath, "Record_*.mp4"));
            var camera = ARCamera;
            var background = camera.GetComponent<ARCameraBackground>();
            // The recorder renders the AR camera into its own texture; check each such frame as it starts rendering.
            void OnRecordedFrame(ScriptableRenderContext context, Camera rendering)
            {
                if (rendering != camera || rendering.targetTexture == null) return;
                if (!ARKitCameraReference.TryGetReference(camera, out var rotation, out _, out var display)) return;
                var angle = Quaternion.Angle(camera.transform.localRotation, rotation);
                var used = background.material.GetMatrix("_UnityDisplayTransform");
                var error = 0f;
                for (var i = 0; i < 16; i++) error = Mathf.Max(error, Mathf.Abs(used[i] - display[i]));
                if (_renderingInUpdate)
                {
                    _report.updateTimeFramesChecked++;
                    _report.updateTimeCameraAngle = Mathf.Max(_report.updateTimeCameraAngle, angle);
                    _report.updateTimeBackgroundError = Mathf.Max(_report.updateTimeBackgroundError, error);
                    return;
                }
                _report.recordedFramesChecked++;
                _report.recordedCameraAngle = Mathf.Max(_report.recordedCameraAngle, angle);
                _report.recordedBackgroundError = Mathf.Max(_report.recordedBackgroundError, error);
            }
            RenderPipelineManager.beginCameraRendering += OnRecordedFrame;
            // First render a few frames the way the recorder used to: during Update (see Update below).
            _probeBuffer = HoloKitVideoRecorder.CreateCameraBuffer(Screen.width, Screen.height);
            _probeFrame = new RenderTexture(Screen.width, Screen.height, 0);
            _updateTimeRenders = 10;
            while (_updateTimeRenders > 0) yield return null;
            Destroy(_probeBuffer);
            Destroy(_probeFrame);
            // Settings > Record Microphone (on by default) adds the microphone and the mix tracks, checked on the Mac by
            // Tools/check_recording_audio.py. iOS asks once; the device's operator answers, never the test.
            _report.recordMicrophone = recorder.RecordMicrophone;
            if (recorder.RecordMicrophone)
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            _report.microphoneAuthorized = Application.HasUserAuthorization(UserAuthorization.Microphone);
            recorder.StartRecording();
            // The movie's clock starts with its first frame, at the end of this frame.
            yield return new WaitForEndOfFrame();
            var movieStart = Time.realtimeSinceStartup;
            _report.recordingStarted = recorder.IsRecording;
            Save();
            Require(_report.recordingStarted, "Recording did not start.");
            for (var i = 0; i < 5; i++)
            {
                MofaWatchConnectivityAPI.ReceiveWatchTriggerForDeviceTest();
                yield return new WaitForSecondsRealtime(1.1f);
                if (i % 2 == 1)
                {
                    yield return new WaitForEndOfFrame();
                    var shot = $"recording-shot-{i}.png";
                    _report.recordingShotSeconds.Add(Time.realtimeSinceStartup - movieStart);
                    Capture(shot);
                    _report.recordingShots.Add(shot);
                }
            }
            RenderPipelineManager.beginCameraRendering -= OnRecordedFrame;
            Save();
            recorder.EndRecording();
            string file = null;
            long size = -1;
            for (var i = 0; i < 40; i++)
            {
                yield return new WaitForSecondsRealtime(.25f);
                file = Directory.GetFiles(Application.temporaryCachePath, "Record_*.mp4").FirstOrDefault(f => !earlier.Contains(f));
                if (file == null) continue;
                var now = new FileInfo(file).Length;
                if (now > 0 && now == size) break;
                size = now;
            }
            Require(file != null, "No recording was written to " + Application.temporaryCachePath);
            yield return new WaitForSecondsRealtime(1);
            File.Copy(file, Path.Combine(_directory, "recording.mp4"), true);
            _report.video = "recording.mp4";
            _report.videoBytes = new FileInfo(file).Length;
            // A finished MP4 has its index ("moov"); a movie whose writer never finished does not and will not play.
            var bytes = File.ReadAllBytes(file);
            for (var i = 0; i + 4 <= bytes.Length && !_report.videoFinished; i++)
                _report.videoFinished = bytes[i] == 'm' && bytes[i + 1] == 'o' && bytes[i + 2] == 'o' && bytes[i + 3] == 'v';
            Save();
            Require(_report.videoFinished, "The recording was not finished (no MP4 index); it will not play.");
            Require(_report.recordedFramesChecked > 0, "No recorded frame was rendered.");
            // ARKit's frame can advance between AR Foundation's pose update and the recorded render, so allow a few degrees.
            Require(_report.recordedCameraAngle < 5f,
                $"A recorded frame was rendered with the camera {_report.recordedCameraAngle:F1} deg from ARKit's camera.");
            Require(_report.recordedBackgroundError < .05f,
                $"A recorded frame's AR background differs from ARKit's display transform by {_report.recordedBackgroundError:F2}.");
        }

        // Runs at the default execution order, as game scripts and HoloKitVideoRecorder.Update did.
        private void Update()
        {
            if (_measuring != null && ARCamera is Camera camera &&
                ARKitCameraReference.TryGetReference(camera, out var reference, out _))
                _measuring.scriptUpdateAngle = Mathf.Max(_measuring.scriptUpdateAngle,
                    Quaternion.Angle(camera.transform.localRotation, reference));
            if (_updateTimeRenders <= 0) return;
            _updateTimeRenders--;
            _renderingInUpdate = true;
            try { HoloKitVideoRecorder.RenderFrame(ARCamera, _probeBuffer, _probeFrame); }
            finally { _renderingInUpdate = false; }
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

        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            // A frame the encoder was not ready for; counted, and judged from the video.
            if (message.StartsWith(AppendFailure)) _report.recordingAppendFailures++;
            else _report.errors.Add(message + "\n" + stackTrace);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
