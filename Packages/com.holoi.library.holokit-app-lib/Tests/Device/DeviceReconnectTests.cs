#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Holoi.Library.HoloKitAppLib;
using Netcode.Transports.NetworkFramework;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// A client whose first connection attempt fails must try again and join, through the app's own flow: the host
    /// enters MOFA Duel and shows its QR panel (which advertises), the client enters it as a player and waits on the
    /// app's waiting-for-connection panel. The host rejects the first connection request, as a stalled attempt that
    /// reaches the handshake timeout ends too, and approves the next ones. No camera or QR scan is needed.
    /// Run with HOLOKIT_RECONNECT_ROLE=host on one phone and =client on the other.
    /// </summary>
    public sealed class DeviceReconnectTests : MonoBehaviour
    {
        private const string Reality = "MOFATheDuel";
        private const float JoinTimeoutSeconds = 120f;

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string role;
            public bool complete;
            public int requestsRejected, requestsApproved;
            public bool clientJoinedHost;
            public int connectAttempts;
            public bool joined;
            public bool connectedClient, multiplayerManagerPresent, multiplayerManagerSpawned;
            public float joinSeconds;
            public string panelText;
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("HOLOKIT_RECONNECT_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("HoloKit Reconnect Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceReconnectTests>()._report.role = role;
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _directory = Path.Combine(Application.persistentDataPath, "ReconnectTests");
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
            Debug.Log($"[RECONNECT TEST] role={_report.role} complete={_report.complete} joined={_report.joined} " +
                      $"attempts={_report.connectAttempts} rejected={_report.requestsRejected} errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            for (var i = 0; i < 100 && HoloKitApp.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return new WaitForSecondsRealtime(1f);
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == Reality);
            Require(reality != null, Reality + " is missing from the catalog.");
            app.CurrentReality = reality;
            if (_report.role == "host")
            {
                // The app approves every request; the host here decides itself, so it can turn the first one away.
                var field = typeof(HoloKitApp).GetField("_networkManagerPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
                var prefab = (NetworkManager)field.GetValue(app);
                prefab.GetComponent<NetworkFrameworkTransport>().AutoApproveConnectionRequest = false;
                app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            }
            else
            {
                app.EnterRealityAs(false, HoloKitAppPlayerType.Player);
            }
            // Stepped here rather than yielded to Unity, so a failed check reaches Start's catch and the report.
            var steps = _report.role == "host" ? Host(app) : Client(app);
            while (steps.MoveNext())
                yield return steps.Current;
        }

        private IEnumerator Host(HoloKitApp app)
        {
            for (var i = 0; i < 100 && NetworkFrameworkTransport.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var transport = NetworkFrameworkTransport.Instance;
            Require(transport != null && !transport.AutoApproveConnectionRequest, "The host did not take manual approval.");
            transport.OnConnectionRequest += (key, _) =>
            {
                if (_report.requestsRejected == 0) { transport.RejectConnectionRequest(key); _report.requestsRejected++; }
                else { transport.ApproveConnectionRequest(key); _report.requestsApproved++; }
                Save();
            };
            yield return new WaitForSecondsRealtime(3f);
            app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode"); // Advertises, as the spectator button does.
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalSeconds < JoinTimeoutSeconds + 30 && !_report.clientJoinedHost)
            {
                _report.clientJoinedHost = NetworkManager.Singleton != null && NetworkManager.Singleton.ConnectedClientsIds.Count >= 2;
                yield return new WaitForSecondsRealtime(.2f);
            }
            yield return new WaitForSecondsRealtime(5f); // Let the client see itself joined before the host leaves.
            Require(_report.requestsRejected == 1, "The client never asked to connect.");
            Require(_report.clientJoinedHost, "The client did not join after its first request was rejected.");
        }

        private IEnumerator Client(HoloKitApp app)
        {
            for (var i = 0; i < 100 && NetworkFrameworkTransport.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var transport = NetworkFrameworkTransport.Instance;
            Require(transport != null, "The client's transport did not start.");
            transport.OnConnectingToHost += _ => { _report.connectAttempts++; Save(); };
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalSeconds < JoinTimeoutSeconds && !_report.joined)
            {
                var manager = NetworkManager.Singleton;
                _report.joined = manager != null && manager.IsConnectedClient && app.MultiplayerManager != null &&
                                 app.MultiplayerManager.IsSpawned && app.MultiplayerManager.LocalPlayer != null;
                yield return new WaitForSecondsRealtime(.2f);
            }
            _report.joinSeconds = (float)Math.Round(clock.Elapsed.TotalSeconds, 1);
            _report.connectedClient = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
            _report.multiplayerManagerPresent = app.MultiplayerManager != null;
            _report.multiplayerManagerSpawned = _report.multiplayerManagerPresent && app.MultiplayerManager.IsSpawned;
            _report.panelText = FindObjectsByType<TMPro.TMP_Text>(FindObjectsSortMode.None)
                .Where(t => t.isActiveAndEnabled && t.text.Contains("host")).Select(t => t.text.Replace("\n", " ")).FirstOrDefault();
            Require(_report.connectAttempts >= 1, "The client never started connecting to the host.");
            Require(_report.joined, $"The client did not join within {JoinTimeoutSeconds} s after {_report.connectAttempts} " +
                                    $"attempt(s); the panel shows \"{_report.panelText}\".");
            Require(_report.connectAttempts >= 2, "The client joined on its first attempt; the host did not reject it.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
