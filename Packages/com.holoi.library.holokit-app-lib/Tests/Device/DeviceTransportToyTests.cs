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
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Two iPhones connect through the app's own Network Manager prefab in an empty toy scene, with no AR, QR code or
    /// person: the host advertises the way the QR sharing panel does and the client browses the way the
    /// waiting-for-connection panel does. The client joins, exchanges reliable and unreliable round trips and 60 Hz
    /// poses, and leaves, Cycles times, as a player entering and leaving realities would.
    /// Run with HOLOKIT_TOY_TRANSPORT_ROLE=host on one phone and =client on the other.
    /// </summary>
    public sealed class DeviceTransportToyTests : MonoBehaviour
    {
        private const string SessionId = "holokit-toy-transport";
        private const int Cycles = 5;
        private const int Pings = 30;
        private const float ConnectTimeoutSeconds = 20f;

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string operatingSystem;
            public string product;
            public string role;
            public string transport;
            public bool platformSupported;
            public bool complete;
            public int cycles = Cycles;
            public int connected;
            public List<float> connectSeconds = new();
            public int reliableSent, reliableReceived, unreliableSent, unreliableReceived;
            public float reliableRttP50Ms, reliableRttP95Ms, unreliableRttP50Ms, unreliableRttP95Ms;
            public int posesSent;
            public int hostConnections, hostDisconnects, hostPosesReceived;
            public bool hostSawClientFinish;
            public List<string> errors = new();
            public List<string> otherErrorLogs = new();
        }

        private readonly Report _report = new();
        private readonly List<double> _reliableRtt = new();
        private readonly List<double> _unreliableRtt = new();
        private readonly Dictionary<(int reliable, int seq), Stopwatch> _pending = new();
        private string _directory;
        private Scene _toy;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("HOLOKIT_TOY_TRANSPORT_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("HoloKit Toy Transport Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceTransportToyTests>()._report.role = role;
        }

        private IEnumerator Start()
        {
            Debug.unityLogger.logEnabled = true;
            Application.logMessageReceived += OnLog;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _directory = Path.Combine(Application.persistentDataPath, "ToyTransportTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.operatingSystem = SystemInfo.operatingSystem;
            _report.product = Application.identifier;
            _report.platformSupported = NetworkFrameworkTransport.IsPlatformSupported;
            Save();
            var run = _report.role == "host" ? Host() : Client();
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
            Application.logMessageReceived -= OnLog;
            Debug.Log($"[TOY TRANSPORT] role={_report.role} complete={_report.complete} connected={_report.connected}/{Cycles} " +
                      $"hostConnections={_report.hostConnections} errors={_report.errors.Count}");
        }

        // The app's Network Manager, set up as HoloKitApp.InitializeNetworkManager does on a device. The toy scene has no
        // reality, so it has no player prefab and no scene synchronisation.
        private NetworkFrameworkTransport CreateManager()
        {
            var field = typeof(HoloKitApp).GetField("_networkManagerPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            Require(field != null && HoloKitApp.Instance != null, "HoloKitApp or its Network Manager prefab is missing.");
            var manager = Instantiate((NetworkManager)field.GetValue(HoloKitApp.Instance));
            Destroy(manager.GetComponent<UnityTransport>());
            manager.NetworkConfig.PlayerPrefab = null;
            manager.NetworkConfig.EnableSceneManagement = false;
            var transport = manager.NetworkConfig.NetworkTransport as NetworkFrameworkTransport;
            Require(transport != null, "The app's Network Manager does not use NetworkFrameworkTransport: " +
                                       manager.NetworkConfig.NetworkTransport?.GetType().FullName);
            _report.transport = transport.GetType().FullName;
            transport.SessionId = SessionId;
            return transport;
        }

        private IEnumerator OpenToyScene()
        {
            for (var i = 0; i < 100 && HoloKitApp.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(1f);
            _toy = SceneManager.CreateScene("TransportToy");
            SceneManager.SetActiveScene(_toy);
        }

        private IEnumerator Host()
        {
            yield return OpenToyScene();
            var transport = CreateManager();
            var manager = NetworkManager.Singleton;
            manager.OnClientConnectedCallback += id => { if (id != NetworkManager.ServerClientId) _report.hostConnections++; Save(); };
            manager.OnClientDisconnectCallback += id => { if (id != NetworkManager.ServerClientId) _report.hostDisconnects++; Save(); };
            Require(manager.StartHost(), "StartHost failed.");
            transport.StartAdvertising(); // As the QR sharing panel does (AutoAdvertise is off).
            var messages = manager.CustomMessagingManager;
            messages.RegisterNamedMessageHandler("toy-ping", (sender, reader) =>
            {
                reader.ReadValueSafe(out int seq);
                reader.ReadValueSafe(out int reliable);
                Send(manager, sender, "toy-pong", reliable == 1 ? NetworkDelivery.Reliable : NetworkDelivery.Unreliable, seq, reliable);
            });
            messages.RegisterNamedMessageHandler("toy-pose", (_, _) => _report.hostPosesReceived++);
            messages.RegisterNamedMessageHandler("toy-done", (_, _) => _report.hostSawClientFinish = true);
            var clock = Stopwatch.StartNew();
            while (!_report.hostSawClientFinish && clock.Elapsed.TotalSeconds < 300)
            {
                if ((int)clock.Elapsed.TotalSeconds % 5 == 0) Save();
                yield return null;
            }
            yield return new WaitForSecondsRealtime(2f);
            manager.Shutdown();
            Require(_report.hostSawClientFinish, "The client never finished its cycles.");
            Require(_report.hostConnections >= Cycles, $"Only {_report.hostConnections} of {Cycles} client connections reached the host.");
        }

        private IEnumerator Client()
        {
            yield return OpenToyScene();
            for (var cycle = 1; cycle <= Cycles; cycle++)
            {
                var transport = CreateManager();
                var manager = NetworkManager.Singleton;
                var clock = Stopwatch.StartNew();
                Require(manager.StartClient(), "StartClient failed.");
                transport.StartBrowsing(); // As the waiting-for-connection panel does (AutoBrowse is off).
                while (!manager.IsConnectedClient && clock.Elapsed.TotalSeconds < ConnectTimeoutSeconds)
                    yield return null;
                if (manager.IsConnectedClient)
                {
                    _report.connected++;
                    _report.connectSeconds.Add((float)Math.Round(clock.Elapsed.TotalSeconds, 2));
                    manager.CustomMessagingManager.RegisterNamedMessageHandler("toy-pong", (_, reader) =>
                    {
                        reader.ReadValueSafe(out int seq);
                        reader.ReadValueSafe(out int reliable);
                        if (!_pending.Remove((reliable, seq), out var sent)) return;
                        (reliable == 1 ? _reliableRtt : _unreliableRtt).Add(sent.Elapsed.TotalMilliseconds);
                        if (reliable == 1) _report.reliableReceived++; else _report.unreliableReceived++;
                    });
                    foreach (var reliable in new[] { 1, 0 })
                        for (var i = 0; i < Pings; i++)
                        {
                            var seq = cycle * 1000 + i;
                            _pending[(reliable, seq)] = Stopwatch.StartNew();
                            Send(manager, NetworkManager.ServerClientId, "toy-ping",
                                reliable == 1 ? NetworkDelivery.Reliable : NetworkDelivery.Unreliable, seq, reliable);
                            if (reliable == 1) _report.reliableSent++; else _report.unreliableSent++;
                            yield return new WaitForSecondsRealtime(.05f);
                        }
                    var poses = Stopwatch.StartNew();
                    while (poses.Elapsed.TotalSeconds < 2)
                    {
                        Send(manager, NetworkManager.ServerClientId, "toy-pose", NetworkDelivery.Unreliable, _report.posesSent++, 0);
                        yield return new WaitForSecondsRealtime(1f / 60f);
                    }
                    yield return new WaitForSecondsRealtime(2f); // Late pongs.
                    if (cycle == Cycles)
                    {
                        Send(manager, NetworkManager.ServerClientId, "toy-done", NetworkDelivery.Reliable, cycle, 1);
                        yield return new WaitForSecondsRealtime(1f);
                    }
                }
                else
                {
                    _report.errors.Add($"Cycle {cycle}: not connected within {ConnectTimeoutSeconds} s.");
                }
                Save();
                manager.Shutdown();
                while (manager.ShutdownInProgress) yield return null;
                Destroy(manager.gameObject);
                yield return new WaitForSecondsRealtime(1f);
            }
            Percentiles(_reliableRtt, out _report.reliableRttP50Ms, out _report.reliableRttP95Ms);
            Percentiles(_unreliableRtt, out _report.unreliableRttP50Ms, out _report.unreliableRttP95Ms);
            Require(_report.connected == Cycles, $"Connected in {_report.connected} of {Cycles} cycles.");
            Require(_report.reliableReceived == _report.reliableSent,
                $"Lost {_report.reliableSent - _report.reliableReceived} of {_report.reliableSent} reliable round trips.");
            Require(_report.unreliableReceived >= _report.unreliableSent * 0.8f,
                $"Unreliable round trips: {_report.unreliableReceived} of {_report.unreliableSent} came back.");
        }

        private static void Send(NetworkManager manager, ulong to, string name, NetworkDelivery delivery, int seq, int flag)
        {
            using var writer = new FastBufferWriter(8, Allocator.Temp);
            writer.WriteValueSafe(seq);
            writer.WriteValueSafe(flag);
            manager.CustomMessagingManager.SendNamedMessage(name, to, writer, delivery);
        }

        private static void Percentiles(List<double> samples, out float p50, out float p95)
        {
            var sorted = samples.OrderBy(s => s).ToList();
            p50 = sorted.Count == 0 ? -1 : (float)sorted[sorted.Count / 2];
            p95 = sorted.Count == 0 ? -1 : (float)sorted[Math.Min(sorted.Count - 1, (int)(sorted.Count * .95))];
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        // Transport and Netcode errors fail the run; the app's own (permissions, watch) are kept for reading.
        private void OnLog(string message, string stack, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            var transportError = message.Contains("NetworkFramework") || message.Contains("[Netcode]") || type == LogType.Exception;
            (transportError ? _report.errors : _report.otherErrorLogs).Add(message);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
