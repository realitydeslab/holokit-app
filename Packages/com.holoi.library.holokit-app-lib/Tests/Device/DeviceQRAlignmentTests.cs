#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Measures how well a spectator lines up with the host after scanning its QR code. The host enters a reality,
    /// shows its code and logs, by the network clock, where it believes the code is (its camera pose plus the code's
    /// offset from the camera), along with its screen geometry. The client joins as a spectator and, for several rounds,
    /// scans the code, saves the image-pose pairs the alignment was solved from and the result, photographs its screen
    /// with the alignment marker, and then tracks the code again in the aligned world; between rounds it taps the app's
    /// rescan button. Where the client sees the code after aligning, against where the host says it is, is the alignment
    /// error (Tools/analyze_qr_alignment.py). The client's camera must see the host's screen.
    /// HOLOKIT_QR_ALIGNMENT_ROLE=host with HOLOKIT_QR_ALIGNMENT_CLIENTS=N, or =client with HOLOKIT_CLIENT_INDEX;
    /// HOLOKIT_QR_ALIGNMENT_SCENE picks the reality (default: the first), HOLOKIT_QR_ALIGNMENT_ROUNDS the rounds (3).
    /// HOLOKIT_QR_ALIGNMENT_TURN=degrees turns a client's world about up before it scans, as if it had started its AR
    /// session facing that much further round than the host: the alignment must take the turn out.
    /// </summary>
    public sealed class DeviceQRAlignmentTests : MonoBehaviour
    {
        private const float EnterTimeoutSeconds = 20f;
        private const float SyncTimeoutSeconds = 150f;
        private const float RetrackSeconds = 4f;
        private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string PoseColumns = "x,y,z,qx,qy,qz,qw";

        [Serializable]
        private sealed class HostGeometry
        {
            public int screenWidth, screenHeight;
            public float unityDpi, profileDpi, canvasScaleFactor, qrSizeUnits;
            public string unityOrientation, interfaceOrientation, canvasMode, qrCorners;
            public Vector3 phoneCameraOffset, cameraToQRCodeOffset, cameraToScreenCenterOffset;
        }

        [Serializable]
        private sealed class Round
        {
            public int round;
            public bool synced;
            public float seconds, thetaDeg, thetaSpreadDeg;
            public Vector3 translate;
            public int pairs, retracked;
        }

        [Serializable]
        private sealed class Report
        {
            public string device, role, scene;
            // Every run's report differs from the last one, which the device runner waits for.
            public string started = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            public int expectedClients = 1, clientIndex = 1, roundsWanted = 3;
            public float turnDeg;
            public bool complete;
            public HostGeometry host;
            public List<Round> rounds = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;
        private Matrix4x4 _hostCameraPose;
        private double _hostCameraTimestamp;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("HOLOKIT_QR_ALIGNMENT_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("HoloKit QR Alignment Tests");
            DontDestroyOnLoad(runner);
            var tests = runner.AddComponent<DeviceQRAlignmentTests>();
            tests._report.role = role;
            if (int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_QR_ALIGNMENT_CLIENTS"), out var clients))
                tests._report.expectedClients = clients;
            if (int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_CLIENT_INDEX"), out var index))
                tests._report.clientIndex = index;
            if (int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_QR_ALIGNMENT_ROUNDS"), out var rounds))
                tests._report.roundsWanted = rounds;
            if (float.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_QR_ALIGNMENT_TURN"), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var turn))
                tests._report.turnDeg = turn;
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _directory = Path.Combine(Application.persistentDataPath, "QRAlignmentTests");
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
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnHostFrame;
            Save();
            Debug.Log($"[QR ALIGNMENT TEST] role={_report.role} complete={_report.complete} " +
                      $"rounds={_report.rounds.Count(r => r.synced)} errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            for (var i = 0; i < 100 && HoloKitApp.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return new WaitForSecondsRealtime(2f);
            var wanted = Environment.GetEnvironmentVariable("HOLOKIT_QR_ALIGNMENT_SCENE");
            var reality = app.GlobalSettings.GetAllRealities()
                .FirstOrDefault(r => string.IsNullOrEmpty(wanted) || r.Scene.SceneName == wanted);
            Require(reality != null, $"No reality named {wanted}.");
            _report.scene = reality.Scene.SceneName;
            var host = _report.role == "host";
            app.CurrentReality = reality;
            app.EnterRealityAs(host, host ? HoloKitAppPlayerType.Player : HoloKitAppPlayerType.Spectator);
            for (var until = Time.realtimeSinceStartup + EnterTimeoutSeconds; Time.realtimeSinceStartup < until &&
                 !(SceneManager.GetActiveScene().name == _report.scene && NetworkManager.Singleton != null);)
                yield return new WaitForSecondsRealtime(.2f);
            Require(SceneManager.GetActiveScene().name == _report.scene, "The reality did not open.");
            var steps = host ? Host(app) : Client(app);
            while (steps.MoveNext())
                yield return steps.Current;
        }

        private IEnumerator Host(HoloKitApp app)
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnHostFrame;
            yield return new WaitForSecondsRealtime(3f);
            app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode"); // As the spectator button does; this advertises.
            yield return new WaitForSecondsRealtime(1.5f);
            _report.host = MeasureHost(app);
            Save();
            yield return new WaitForEndOfFrame();
            Capture("host-screen.png");
            // Where the host believes its code is, computed as the host does when a client asks.
            var offset = _report.host.cameraToQRCodeOffset;
            var log = new StringBuilder($"serverTime,cam{PoseColumns.Replace(",", ",cam")},qr{PoseColumns.Replace(",", ",qr")}\n");
            var logged = 0.0;
            var expected = 1 + _report.expectedClients;
            var timeout = SyncTimeoutSeconds + 60f * _report.roundsWanted;
            for (var until = Time.realtimeSinceStartup + timeout;
                 Time.realtimeSinceStartup < until && !EveryoneChecked(app, expected);)
            {
                if (_hostCameraTimestamp > logged)
                {
                    logged = _hostCameraTimestamp;
                    var rotation = _hostCameraPose.rotation;
                    var position = _hostCameraPose.GetPosition();
                    log.Append(Number(NetworkManager.Singleton.ServerTime.Time)).Append(',')
                       .Append(Pose(position, rotation)).Append(',')
                       .Append(Pose(position + rotation * offset, rotation * Quaternion.Euler(-90f, 0f, 0f))).Append('\n');
                }
                yield return null;
            }
            File.WriteAllText(Path.Combine(_directory, "host-qr.csv"), log.ToString());
            Require(EveryoneChecked(app, expected), "Not every client finished its rounds and confirmed the alignment.");
            DeviceTaps.CloseShareQRCode();
            yield return new WaitForSecondsRealtime(3f);
        }

        private void OnHostFrame(double timestamp, Matrix4x4 pose)
        {
            _hostCameraTimestamp = timestamp;
            _hostCameraPose = pose;
        }

        private static HostGeometry MeasureHost(HoloKitApp app)
        {
            var manager = app.MultiplayerManager;
            var panel = FindObjectsByType<HoloKitAppUIPanel_MonoAR_ShareQRCode>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.isActiveAndEnabled);
            var qr = panel == null ? null :
                typeof(HoloKitAppUIPanel_MonoAR_ShareQRCode).GetField("_qrCode", Private)?.GetValue(panel) as RectTransform;
            var corners = new Vector3[4];
            if (qr != null) qr.GetWorldCorners(corners);
            var canvas = qr == null ? null : qr.GetComponentInParent<Canvas>().rootCanvas;
            return new HostGeometry
            {
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                unityDpi = Screen.dpi,
                profileDpi = HoloKitOpticsAPI.GetScreenDpi(),
                canvasScaleFactor = canvas != null ? canvas.scaleFactor : 0f,
                canvasMode = canvas != null ? canvas.renderMode.ToString() : "none",
                qrSizeUnits = qr != null ? qr.sizeDelta.x : 0f,
                qrCorners = string.Join(" ", corners.Select(c => $"({c.x:F0},{c.y:F0})")),
                unityOrientation = Screen.orientation.ToString(),
                interfaceOrientation = InterfaceOrientation(),
                phoneCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX),
                cameraToQRCodeOffset = (Vector3)typeof(HoloKitAppMultiplayerManager)
                    .GetField("_cameraToQRCodeOffset", Private).GetValue(manager),
                cameraToScreenCenterOffset = manager.HostCameraToScreenCenterOffset.Value
            };
        }

        private static string InterfaceOrientation()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return HoloKit.iOS.ARKitCameraReference.InterfaceOrientation.ToString();
#else
            return "n/a";
#endif
        }

        private IEnumerator Client(HoloKitApp app)
        {
            var manager = app.MultiplayerManager;
            if (_report.turnDeg != 0f)
            {
                // Once the AR session runs, and before the code is scanned (finding the host and syncing clocks
                // takes several seconds more).
                yield return new WaitForSecondsRealtime(1.5f);
                HoloKitARSessionControllerAPI.ResetOrigin(Vector3.zero, Quaternion.AngleAxis(_report.turnDeg, Vector3.up));
            }
            for (var number = 1; number <= _report.roundsWanted; number++)
            {
                var round = new Round { round = number };
                _report.rounds.Add(round);
                var started = Time.realtimeSinceStartup;
                if (number > 1)
                {
                    // Rescan as a person does from the alignment marker panel.
                    var panel = FindObjectsByType<HoloKitAppUIPanel_MonoAR_CheckAlignmentMark>(FindObjectsSortMode.None)
                        .FirstOrDefault(p => p.isActiveAndEnabled);
                    Require(panel != null && DeviceTaps.Tap(panel,
                            nameof(HoloKitAppUIPanel_MonoAR_CheckAlignmentMark.OnRescanButtonPressed)),
                        $"The rescan button cannot be tapped (covered by {DeviceTaps.LastBlocker}).");
                    yield return null;
                }
                for (var until = Time.realtimeSinceStartup + SyncTimeoutSeconds;
                     Time.realtimeSinceStartup < until && manager.CurrentStatus != HoloKitAppPlayerStatus.Synced;)
                    yield return new WaitForSecondsRealtime(.1f);
                round.synced = manager.CurrentStatus == HoloKitAppPlayerStatus.Synced;
                round.seconds = (float)Math.Round(Time.realtimeSinceStartup - started, 1);
                SavePairs(manager, round); // Also when it did not align: how many pairs came, how far theta wandered.
                Save();
                Require(round.synced, $"Round {number} did not align (status {manager.CurrentStatus}, " +
                                      $"{round.pairs} pairs, theta spread {round.thetaSpreadDeg:F2} deg).");
                yield return new WaitForSecondsRealtime(1f); // The alignment marker is placed and drawn.
                yield return new WaitForEndOfFrame();
                Capture($"marker-{number}.png");
                var retrack = Retrack(app, round);
                while (retrack.MoveNext())
                    yield return retrack.Current;
                Save();
            }
            var tapped = false;
            for (var i = 0; i < 100 && !tapped; i++)
            {
                yield return new WaitForSecondsRealtime(.1f);
                tapped = DeviceTaps.TapCheckMark();
            }
            Require(tapped, $"The check button cannot be tapped (covered by {DeviceTaps.LastBlocker}).");
            for (var until = Time.realtimeSinceStartup + 60f; Time.realtimeSinceStartup < until &&
                 NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;)
                yield return new WaitForSecondsRealtime(.2f);
        }

        // The image-pose pairs the alignment was solved from (the last ones, at most 51) and the solved result.
        private void SavePairs(HoloKitAppMultiplayerManager manager, Round round)
        {
            var type = typeof(HoloKitAppMultiplayerManager);
            var pairs = ((IEnumerable<ImagePosePair>)type.GetField("_imagePosePairQueue", Private).GetValue(manager)).ToList();
            var results = ((IEnumerable<SyncResult>)type.GetField("_syncResultQueue", Private).GetValue(manager)).ToList();
            round.pairs = pairs.Count;
            if (results.Count > 0)
            {
                round.thetaDeg = results[^1].ThetaInDeg;
                round.translate = results[^1].Translate;
                round.thetaSpreadDeg = results.Max(r => r.ThetaInDeg) - results.Min(r => r.ThetaInDeg);
            }
            var csv = new StringBuilder($"host{PoseColumns.Replace(",", ",host")},client{PoseColumns.Replace(",", ",client")}\n");
            foreach (var pair in pairs)
                csv.Append(Pose(pair.HostImagePosition, pair.HostImageRotation)).Append(',')
                   .Append(Pose(pair.ClientImagePosition, pair.ClientImageRotation)).Append('\n');
            File.WriteAllText(Path.Combine(_directory, $"pairs-{round.round}.csv"), csv.ToString());
        }

        // Tracks the host's code again after aligning: in the aligned world it should be where the host says it is.
        private IEnumerator Retrack(HoloKitApp app, Round round)
        {
            var images = app.ARSessionManager.ARTrackedImageManager;
            var csv = new StringBuilder($"serverTime,{PoseColumns},sizeX,sizeY\n");
            var count = 0;
            UnityAction<ARTrackablesChangedEventArgs<ARTrackedImage>> listener = args =>
            {
                foreach (var image in args.added.Concat(args.updated))
                {
                    if (image.trackingState != TrackingState.Tracking) continue;
                    var t = image.transform;
                    csv.Append(Number(NetworkManager.Singleton.ServerTime.Time)).Append(',')
                       .Append(Pose(t.position, t.rotation)).Append(',')
                       .Append(Number(image.size.x)).Append(',').Append(Number(image.size.y)).Append('\n');
                    count++;
                }
            };
            images.trackablesChanged.AddListener(listener);
            app.ARSessionManager.SetARTrackedImageManagerEnabled(true);
            yield return new WaitForSecondsRealtime(RetrackSeconds);
            app.ARSessionManager.SetARTrackedImageManagerEnabled(false);
            images.trackablesChanged.RemoveListener(listener);
            round.retracked = count;
            File.WriteAllText(Path.Combine(_directory, $"retrack-{round.round}.csv"), csv.ToString());
        }

        private static bool EveryoneChecked(HoloKitApp app, int expected)
        {
            var players = app.MultiplayerManager != null ? app.MultiplayerManager.PlayerList : null;
            return players != null && NetworkManager.Singleton != null &&
                   NetworkManager.Singleton.ConnectedClientsIds.Count >= expected &&
                   players.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked);
        }

        private static string Number(double value) => value.ToString("F6", CultureInfo.InvariantCulture);

        private static string Pose(Vector3 p, Quaternion q) => string.Join(",",
            new double[] { p.x, p.y, p.z, q.x, q.y, q.z, q.w }.Select(Number));

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

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
