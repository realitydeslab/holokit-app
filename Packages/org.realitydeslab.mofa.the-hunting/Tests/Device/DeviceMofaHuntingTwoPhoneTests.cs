#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.TheHunting.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheHunting
{
    // Run on two physical iPhones with MOFA_HUNTING_TWO_PHONE_ROLE=host/client.
    public sealed class DeviceMofaHuntingTwoPhoneTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public string device;
            public string role;
            public bool complete;
            public bool arTracking;
            public bool networkListening;
            public int playerCount;
            public string localStatus;
            public bool connected;
            public bool spatialSynced;
            public bool dragonSpawned;
            public bool portalSpawned;
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("MOFA_HUNTING_TWO_PHONE_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("MOFA Hunting Two Phone Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaHuntingTwoPhoneTests>();
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "MofaHuntingTwoPhoneTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.role = Environment.GetEnvironmentVariable("MOFA_HUNTING_TWO_PHONE_ROLE");
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
            Application.logMessageReceived -= OnLog;
            Debug.Log($"[MOFA HUNTING TWO PHONE] role={_report.role} complete={_report.complete} " +
                      $"players={_report.playerCount} synced={_report.spatialSynced} " +
                      $"dragon={_report.dragonSpawned} errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission is required.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheHunting");
            Require(reality != null, "Hunting is missing from the product catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(_report.role == "host", HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(5);
            Require(SceneManager.GetActiveScene().name == "MOFATheHunting", "Hunting scene did not open.");
            var hunting = app.RealityManager as MofaHuntingRealityManager;
            Require(hunting != null, "Hunting reality manager did not initialize.");
            if (_report.role == "host")
            {
                app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
                Debug.Log("[MOFA HUNTING TWO PHONE] Host QR sharing panel opened.");
            }

            for (var i = 0; i < 900; i++)
            {
                Refresh(app);
                if (i % 20 == 0) Save();
                if (_report.connected && _report.arTracking &&
                    (_report.role == "host" || _report.spatialSynced)) break;
                yield return new WaitForSecondsRealtime(.1f);
            }
            Refresh(app);
            Save();
            Capture("connection.png");
            Require(_report.connected, "Two iPhones did not connect through the app's Network framework transport.");
            if (_report.role == "client")
            {
                Require(_report.spatialSynced, "Client did not complete QR spatial alignment.");
                app.MultiplayerManager.OnCheckAlignmentMarker();
            }
            else
            {
                for (var i = 0; i < 250 &&
                    !app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked); i++)
                    yield return new WaitForSecondsRealtime(.1f);
                Require(app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked),
                    "Client did not confirm the alignment marker.");
                app.UIPanelManager.PopUIPanel();
                var button = FindObjectsOfType<MofaHuntingDragonControllerUIPanel>(true).FirstOrDefault();
                Require(button != null, "Dragon summon button is missing.");
                button.OnSpawnDragonButtonPressedFunc();
            }
            for (var i = 0; i < 300 && (hunting.DragonController == null || hunting.PortalController == null); i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.dragonSpawned = hunting.DragonController != null;
            _report.portalSpawned = hunting.PortalController != null;
            Save();
            Capture("dragon-networked.png");
            Require(_report.dragonSpawned && _report.portalSpawned,
                "Dragon and portal did not replicate to both iPhones.");
            Require(_report.errors.Count == 0, "Hunting logged runtime errors: " + _report.errors.Count);
        }

        private void Refresh(HoloKitApp app)
        {
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            _report.networkListening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
            _report.playerCount = app.MultiplayerManager != null ? app.MultiplayerManager.PlayerCount : 0;
            _report.connected = _report.networkListening && _report.playerCount >= 2;
            _report.localStatus = app.MultiplayerManager?.LocalPlayer != null
                ? app.MultiplayerManager.LocalPlayer.PlayerStatus.Value.ToString() : "None";
            _report.spatialSynced = _report.role == "host" ||
                _report.localStatus == HoloKitAppPlayerStatus.Synced.ToString() ||
                _report.localStatus == HoloKitAppPlayerStatus.Checked.ToString();
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
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                _report.errors.Add(message + "\n" + stackTrace);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
