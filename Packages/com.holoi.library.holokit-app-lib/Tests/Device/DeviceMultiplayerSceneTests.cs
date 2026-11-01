#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Holoi.AssetFoundation;
using Holoi.Library.HoloKitAppLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Every reality with a host and several clients, through the app's own flow: the host enters the reality and
    /// shows its QR code; each client enters the same reality (odd clients as players, even ones as spectators), finds
    /// the host, scans the code and taps the check button; the host waits until every client has confirmed, closes its
    /// QR panel and leaves, and each client leaves once the host has gone. All devices walk the reality list in the
    /// same order, and a client only finds a host in the reality it entered, so a reality that fails on one device
    /// does not shift the others. The clients' cameras must see the host's screen.
    /// Run with HOLOKIT_MULTIPLAYER_SCENES_ROLE=host and HOLOKIT_MULTIPLAYER_SCENES_CLIENTS=N on the host, and
    /// HOLOKIT_MULTIPLAYER_SCENES_ROLE=client and HOLOKIT_CLIENT_INDEX=1..N on the clients.
    /// HOLOKIT_MULTIPLAYER_SCENES_ONLY=SceneA,SceneB limits the run to those realities.
    /// </summary>
    public sealed class DeviceMultiplayerSceneTests : MonoBehaviour
    {
        private const float EnterTimeoutSeconds = 20f;
        private const float JoinTimeoutSeconds = 150f;
        private const float SampleSeconds = 8f;
        private const float HoldSeconds = 2f;

        [Serializable]
        private sealed class SceneResult
        {
            public string scene;
            public string playerType;
            public bool entered, connected, synced, checkMarkTapped, everyoneChecked, realityManagerSpawned, hostLeft;
            public int players;
            public float seconds;
            public string panels;
            public int networkObjects;
            public string error;
        }

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string role;
            public int expectedClients = 1, clientIndex = 1;
            public bool complete;
            public int passed, failed;
            public List<SceneResult> scenes = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("HOLOKIT_MULTIPLAYER_SCENES_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("HoloKit Multiplayer Scene Tests");
            DontDestroyOnLoad(runner);
            var tests = runner.AddComponent<DeviceMultiplayerSceneTests>();
            tests._report.role = role;
            if (int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_MULTIPLAYER_SCENES_CLIENTS"), out var clients))
                tests._report.expectedClients = clients;
            if (int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_CLIENT_INDEX"), out var index))
                tests._report.clientIndex = index;
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _directory = Path.Combine(Application.persistentDataPath, "MultiplayerSceneTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            Save();
            // Each step of the run goes through Step, so a failed check lands in the report instead of escaping.
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
            Debug.Log($"[MULTIPLAYER SCENE TEST] role={_report.role} complete={_report.complete} " +
                      $"passed={_report.passed} failed={_report.failed} errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            for (var i = 0; i < 100 && HoloKitApp.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return new WaitForSecondsRealtime(2f);
            var only = (Environment.GetEnvironmentVariable("HOLOKIT_MULTIPLAYER_SCENES_ONLY") ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
            var realities = app.GlobalSettings.GetAllRealities()
                .Where(r => only.Length == 0 || only.Contains(r.Scene.SceneName)).ToList();
            Require(realities.Count > 0, "No reality to test.");
            var playerType = _report.clientIndex % 2 == 1 ? HoloKitAppPlayerType.Player : HoloKitAppPlayerType.Spectator;
            foreach (var reality in realities)
            {
                var result = new SceneResult
                {
                    scene = reality.Scene.SceneName,
                    playerType = (_report.role == "host" ? HoloKitAppPlayerType.Player : playerType).ToString()
                };
                _report.scenes.Add(result);
                Save();
                var started = Time.realtimeSinceStartup;
                var steps = _report.role == "host" ? Host(app, reality, result) : Client(app, reality, result, playerType);
                while (true)
                {
                    bool more;
                    object next;
                    try { more = steps.MoveNext(); next = more ? steps.Current : null; }
                    catch (Exception exception) { result.error = exception.Message; break; }
                    if (!more) break;
                    yield return next;
                }
                result.seconds = (float)Math.Round(Time.realtimeSinceStartup - started, 1);
                if (result.error == null) _report.passed++;
                else _report.failed++;
                Save();
                // Leave as a player does, and come back to the gallery before the next reality.
                if (SceneManager.GetActiveScene().name != "Start")
                    app.Shutdown();
                for (var until = Time.realtimeSinceStartup + EnterTimeoutSeconds;
                     Time.realtimeSinceStartup < until && SceneManager.GetActiveScene().name != "Start";)
                    yield return new WaitForSecondsRealtime(.2f);
                // The host comes back last, so the clients are already looking for it in the next reality.
                yield return new WaitForSecondsRealtime(_report.role == "host" ? 3f : 1f);
            }
            Require(_report.failed == 0, $"{_report.failed} of {realities.Count} realities failed.");
        }

        private IEnumerator Host(HoloKitApp app, Reality reality, SceneResult result)
        {
            app.CurrentReality = reality;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            for (var until = Time.realtimeSinceStartup + EnterTimeoutSeconds; Time.realtimeSinceStartup < until &&
                 !(InScene(reality) && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening);)
                yield return new WaitForSecondsRealtime(.2f);
            result.entered = InScene(reality) && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
            Require(result.entered, "The reality did not open as host.");
            yield return new WaitForSecondsRealtime(3f);
            app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode"); // As the spectator button does; this advertises.
            var expected = 1 + _report.expectedClients;
            for (var until = Time.realtimeSinceStartup + JoinTimeoutSeconds;
                 Time.realtimeSinceStartup < until && !EveryoneChecked(app, expected);)
                yield return new WaitForSecondsRealtime(.2f);
            result.players = app.MultiplayerManager.PlayerList.Count;
            result.connected = NetworkManager.Singleton.ConnectedClientsIds.Count >= expected;
            result.everyoneChecked = EveryoneChecked(app, expected);
            Require(result.everyoneChecked, $"Not every client joined and confirmed the alignment: {Statuses(app)}.");
            Require(DeviceTaps.CloseShareQRCode(), $"The QR code panel's close button cannot be tapped (covered by " +
                                                   $"{DeviceTaps.LastBlocker}).");
            yield return new WaitForSecondsRealtime(2f);
            result.panels = DeviceTaps.PanelStack();
            result.realityManagerSpawned = app.RealityManager != null && app.RealityManager.IsSpawned;
            Capture(reality);
            Require(result.realityManagerSpawned, "The reality manager is not spawned.");
            // Sample the network objects while the clients do, then give them a moment before ending the session.
            var samples = new StringBuilder(SampleHeader);
            for (var until = Time.realtimeSinceStartup + SampleSeconds; Time.realtimeSinceStartup < until;)
            {
                AppendSample(samples);
                yield return new WaitForSecondsRealtime(.2f);
            }
            SaveSamples(reality, samples, result);
            yield return new WaitForSecondsRealtime(HoldSeconds);
            Require(NetworkManager.Singleton.ConnectedClientsIds.Count >= expected,
                $"A client left before the host did: {Statuses(app)}.");
        }

        private IEnumerator Client(HoloKitApp app, Reality reality, SceneResult result, HoloKitAppPlayerType playerType)
        {
            app.CurrentReality = reality;
            app.EnterRealityAs(false, playerType);
            for (var until = Time.realtimeSinceStartup + EnterTimeoutSeconds;
                 Time.realtimeSinceStartup < until && !InScene(reality);)
                yield return new WaitForSecondsRealtime(.2f);
            result.entered = InScene(reality);
            Require(result.entered, "The reality did not open.");
            // The app's waiting panel finds the host, connects, and opens the QR scanner; the camera sees the host.
            for (var until = Time.realtimeSinceStartup + JoinTimeoutSeconds;
                 Time.realtimeSinceStartup < until && !Synced(app);)
                yield return new WaitForSecondsRealtime(.2f);
            result.connected = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
            result.synced = Synced(app);
            Require(result.connected, "Did not connect to the host.");
            Require(result.synced, $"Did not align with the host's QR code (status {LocalStatus(app)}).");
            for (var i = 0; i < 100 && !result.checkMarkTapped; i++)
            {
                yield return new WaitForSecondsRealtime(.1f);
                result.checkMarkTapped = DeviceTaps.TapCheckMark();
            }
            Require(result.checkMarkTapped, "The alignment marker panel's check button did not appear or cannot be " +
                                            $"tapped (covered by {DeviceTaps.LastBlocker}).");
            for (var i = 0; i < 50 && LocalStatus(app) != HoloKitAppPlayerStatus.Checked.ToString(); i++)
                yield return new WaitForSecondsRealtime(.1f);
            Require(LocalStatus(app) == HoloKitAppPlayerStatus.Checked.ToString(),
                $"The check button did not confirm the alignment (status {LocalStatus(app)}).");
            yield return new WaitForSecondsRealtime(2f);
            result.players = app.MultiplayerManager.PlayerList.Count;
            result.realityManagerSpawned = app.RealityManager != null && app.RealityManager.IsSpawned;
            result.panels = DeviceTaps.PanelStack();
            Capture(reality);
            Require(result.realityManagerSpawned, "The reality manager did not spawn here.");
            var leftover = DeviceTaps.LeftoverJoinPanel();
            Require(leftover == null, $"The {leftover} panel still covers the reality after confirming the alignment " +
                                      $"(panels: {result.panels}).");
            var samples = new StringBuilder(SampleHeader);
            for (var until = Time.realtimeSinceStartup + SampleSeconds; Time.realtimeSinceStartup < until;)
            {
                AppendSample(samples);
                yield return new WaitForSecondsRealtime(.2f);
            }
            SaveSamples(reality, samples, result);
            // Stay in the session until the host ends it.
            for (var until = Time.realtimeSinceStartup + JoinTimeoutSeconds; Time.realtimeSinceStartup < until &&
                 NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;)
                yield return new WaitForSecondsRealtime(.2f);
            result.hostLeft = NetworkManager.Singleton == null || !NetworkManager.Singleton.IsConnectedClient;
            Require(result.hostLeft, "The host never ended the session.");
        }

        private static bool InScene(Reality reality) => SceneManager.GetActiveScene().name == reality.Scene.SceneName;

        // Counts connected devices, not players: a reality may add players of its own (MOFA Training's opponent).
        private static bool EveryoneChecked(HoloKitApp app, int expected)
        {
            var players = app.MultiplayerManager != null ? app.MultiplayerManager.PlayerList : null;
            return players != null && NetworkManager.Singleton != null &&
                   NetworkManager.Singleton.ConnectedClientsIds.Count >= expected &&
                   players.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked);
        }

        private static bool Synced(HoloKitApp app)
        {
            var status = LocalStatus(app);
            return status == HoloKitAppPlayerStatus.Synced.ToString() || status == HoloKitAppPlayerStatus.Checked.ToString();
        }

        private static string LocalStatus(HoloKitApp app) =>
            app.MultiplayerManager != null && app.MultiplayerManager.LocalPlayer != null
                ? app.MultiplayerManager.LocalPlayer.PlayerStatus.Value.ToString() : "None";

        private static string Statuses(HoloKitApp app) => app.MultiplayerManager == null ? "no multiplayer manager" :
            string.Join(", ", app.MultiplayerManager.PlayerList.Select(p =>
                $"{p.PlayerName.Value} {p.PlayerType.Value} {p.PlayerStatus.Value}"));

        private const string SampleHeader = "serverTime,objectId,name,x,y,z\n";

        // Where every network object is by the network's shared clock, so that afterwards each client can be compared
        // with the host: every object the host has should be on the client, where the host has it
        // (Tools/compare_network_objects.py).
        private static void AppendSample(StringBuilder samples)
        {
            var manager = NetworkManager.Singleton;
            if (manager == null || manager.SpawnManager == null)
                return;
            var time = manager.ServerTime.Time;
            foreach (var networkObject in manager.SpawnManager.SpawnedObjectsList)
            {
                if (networkObject == null)
                    continue;
                var position = networkObject.transform.position;
                samples.Append(string.Format(CultureInfo.InvariantCulture, "{0:F3},{1},{2},{3:F3},{4:F3},{5:F3}\n",
                    time, networkObject.NetworkObjectId, networkObject.name.Replace(',', ' '),
                    position.x, position.y, position.z));
            }
        }

        private void SaveSamples(Reality reality, StringBuilder samples, SceneResult result)
        {
            result.networkObjects = NetworkManager.Singleton != null && NetworkManager.Singleton.SpawnManager != null
                ? NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.Count : 0;
            File.WriteAllText(Path.Combine(_directory, $"objects-{reality.Scene.SceneName}.csv"), samples.ToString());
        }

        private void Capture(Reality reality)
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(_directory, reality.Scene.SceneName + ".jpg"), texture.EncodeToJPG(70));
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
