#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using HoloKit.iOS;
using Holoi.Library.HoloKitAppLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheTraining
{
    /// <summary>
    /// Opt-in physical-iPhone check that a recording made in Stereo (MR) plays the right way up. The phone stays still
    /// (best in a HoloKit headset standing upright) while MOFA Training records twice, as a player would: first in
    /// Stereo, opened straight from portrait Mono, then in Mono turned to landscape left, with a screenshot in the middle
    /// (Mono's screen shows the view being recorded). Tools/check_recording_orientation.py compares the Mono movie with
    /// the screenshot and the Stereo movie with the Mono movie, and the recorded camera's up with gravity.
    /// </summary>
    public sealed class DeviceStereoRecordingTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Clip
        {
            public string name;
            public string renderMode;
            public string screenOrientation;
            public string deviceOrientation;
            public string interfaceOrientation;
            public int width;
            public int height;
            public string video;
            public long videoBytes;
            public string screenshot;
            // When the screenshot was taken, in the movie's time.
            public float screenshotSeconds;
            // While recording: the recorded camera's up against gravity (1 upright, -1 upside down, 0 lying flat), and
            // the interface orientation ARKit renders for.
            public float upDot;
            public string recordingInterfaceOrientation;
        }

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string os;
            public string product;
            public bool complete;
            public bool arTracking;
            public List<Clip> clips = new();
            public List<string> errors = new();
        }

        private const string Folder = "MofaStereoRecordingTests";
        private const float ClipSeconds = 3f;
        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (Environment.GetEnvironmentVariable("MOFA_STEREO_RECORDING_TEST") != "1") return;
            var runner = new GameObject("MOFA Stereo Recording Device Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceStereoRecordingTests>();
        }

        private IEnumerator Start()
        {
            var sleep = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, Folder);
            if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.os = SystemInfo.operatingSystem;
            _report.product = Application.identifier;
            Save();
            yield return Section("setup", EnterTraining());
            if (_report.errors.Count == 0)
            {
                // Stereo first, opened from portrait as the app opens it, then Mono in landscape as the reference.
                yield return Section("stereo", Record("stereo", true));
                yield return Section("mono", BackToMono());
                yield return Section("mono-landscape", Record("mono-landscape", false));
            }
            yield return Section("restore", RestoreMono());
            _report.complete = true;
            Save();
            Application.logMessageReceived -= OnLog;
            Screen.sleepTimeout = sleep;
            Debug.Log($"[MOFA STEREO RECORDING TEST] complete errors={_report.errors.Count} report={_directory}/results.json");
            Destroy(gameObject);
        }

        // Runs one part; an exception fails that part and the run moves on.
        private IEnumerator Section(string name, IEnumerator run)
        {
            Debug.Log($"[MOFA STEREO RECORDING TEST] {name}");
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
            for (var i = 0; i < 600 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            Require(_report.arTracking, "AR tracking unavailable: " + ARSession.state);
        }

        // Records one clip in landscape left: Mono turned to landscape left, or Stereo (which turns it there itself).
        private IEnumerator Record(string name, bool stereo)
        {
            var cameraManager = HoloKitCameraManager.Instance;
            if (stereo)
            {
                cameraManager.OpenStereoWithoutNFC("SomethingForNothing");
                for (var i = 0; i < 50 && cameraManager.RenderMode != HoloKitRenderMode.Stereo; i++)
                    yield return new WaitForSecondsRealtime(.1f);
                Require(cameraManager.RenderMode == HoloKitRenderMode.Stereo, "Stereo did not open.");
            }
            else
            {
                cameraManager.ForceScreenOrientation = false;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            for (var i = 0; i < 50 && Screen.width < Screen.height; i++) yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(1.5f);
            var clip = new Clip
            {
                name = name,
                renderMode = cameraManager.RenderMode.ToString(),
                screenOrientation = Screen.orientation.ToString(),
                deviceOrientation = Input.deviceOrientation.ToString(),
                interfaceOrientation = ARKitCameraReference.InterfaceOrientation.ToString(),
                width = Screen.width,
                height = Screen.height
            };
            _report.clips.Add(clip);
            Save();
            Require(Screen.width > Screen.height, $"The screen did not turn landscape: {Screen.width}x{Screen.height}.");

            var recorder = FindObjectOfType<HoloKitVideoRecorder>(true);
            Require(recorder != null, "HoloKitVideoRecorder is missing.");
            // The orientation does not depend on the microphone; leaving it out keeps the run free of permission prompts.
            var recordMicrophone = recorder.RecordMicrophone;
            recorder.RecordMicrophone = false;
            var earlier = new HashSet<string>(Directory.GetFiles(Application.temporaryCachePath, "Record_*.mp4"));
            // As the MR panel's record button and the Mono record button do.
            HoloKitApp.Instance.Recorder.StartRecording();
            // The movie's clock starts with its first frame, at the end of this frame.
            yield return new WaitForEndOfFrame();
            var movieStart = Time.realtimeSinceStartup;
            Require(recorder.IsRecording, "Recording did not start.");
            var recorded = recorder.GetComponent<Camera>();
            var up = 0f;
            var samples = 0;
            while (Time.realtimeSinceStartup - movieStart < ClipSeconds)
            {
                yield return new WaitForEndOfFrame();
                up += Vector3.Dot(recorded.transform.up, Vector3.up);
                samples++;
                if (clip.screenshot == null && Time.realtimeSinceStartup - movieStart >= ClipSeconds / 2)
                {
                    clip.screenshotSeconds = Time.realtimeSinceStartup - movieStart;
                    clip.screenshot = name + ".png";
                    clip.recordingInterfaceOrientation = ARKitCameraReference.InterfaceOrientation.ToString();
                    Capture(clip.screenshot);
                }
            }
            clip.upDot = samples > 0 ? up / samples : 0f;
            HoloKitApp.Instance.Recorder.StopRecording();
            recorder.RecordMicrophone = recordMicrophone;

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
            clip.video = name + ".mp4";
            File.Copy(file, Path.Combine(_directory, clip.video), true);
            clip.videoBytes = new FileInfo(file).Length;
            Save();
        }

        private IEnumerator BackToMono()
        {
            var cameraManager = HoloKitCameraManager.Instance;
            cameraManager.RenderMode = HoloKitRenderMode.Mono;
            for (var i = 0; i < 50 && cameraManager.RenderMode != HoloKitRenderMode.Mono; i++)
                yield return new WaitForSecondsRealtime(.1f);
            Require(cameraManager.RenderMode == HoloKitRenderMode.Mono, "Stereo did not close.");
        }

        private IEnumerator RestoreMono()
        {
            var cameraManager = HoloKitCameraManager.Instance;
            if (cameraManager == null) yield break;
            cameraManager.RenderMode = HoloKitRenderMode.Mono;
            cameraManager.ForceScreenOrientation = true;
            Screen.orientation = ScreenOrientation.Portrait;
            for (var i = 0; i < 50 && Screen.width > Screen.height; i++) yield return new WaitForSecondsRealtime(.1f);
        }

        private void Capture(string name)
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(_directory, name), texture.EncodeToPNG());
            Destroy(texture);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type is LogType.Error or LogType.Exception or LogType.Assert) _report.errors.Add(message + "\n" + stackTrace);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
