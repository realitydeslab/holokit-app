#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using RealityDesignLab.MOFA.Library.Base;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheDuel
{
    // Run on two physical iPhones with MOFA_DUEL_TWO_PHONE_ROLE=host/client.
    // The client's normal MPC discovery and QR image tracking are left intact.
    public sealed class DeviceMofaDuelTwoPhoneTests : MonoBehaviour
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
            public bool fighting;
            public bool lifeShieldReady;
            public float requiredBasicCharge;
            public float basicChargeBeforeShot;
            public int watchTriggers;
            public int localCastsBefore;
            public int localCastsAfter;
            public int remoteCastsSeen;
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("MOFA_DUEL_TWO_PHONE_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("MOFA Duel Two Phone Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaDuelTwoPhoneTests>();
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "MofaDuelTwoPhoneTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.role = Environment.GetEnvironmentVariable("MOFA_DUEL_TWO_PHONE_ROLE");
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
            Debug.Log($"[MOFA DUEL TWO PHONE] role={_report.role} complete={_report.complete} " +
                      $"players={_report.playerCount} synced={_report.spatialSynced} fighting={_report.fighting} " +
                      $"errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission is required.");
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheDuel");
            Require(reality != null, "MOFA Duel is missing from the product catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(_report.role == "host", HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(5);
            Require(SceneManager.GetActiveScene().name == "MOFATheDuel", "Duel scene did not open.");
            var duel = app.RealityManager as MofaDuelRealityManager;
            Require(duel != null, "Duel reality manager did not initialize.");
            if (_report.role == "host")
            {
                // Same QR sharing panel opened by the spectator button.
                app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
                Debug.Log("[MOFA DUEL TWO PHONE] Host QR sharing panel opened.");
            }

            for (var i = 0; i < 900; i++)
            {
                Refresh(app);
                if (i % 20 == 0) Save();
                if (_report.playerCount >= 2 && _report.arTracking &&
                    (_report.role == "host" || _report.localStatus == HoloKitAppPlayerStatus.Synced.ToString() ||
                     _report.localStatus == HoloKitAppPlayerStatus.Checked.ToString()))
                    break;
                yield return new WaitForSecondsRealtime(.1f);
            }
            Refresh(app);
            Save();
            Capture("connection.png");
            Require(_report.connected, "Two iPhones did not connect through the app's MPC transport.");
            if (_report.role == "client")
            {
                Require(_report.spatialSynced, "Client did not complete QR spatial alignment.");
                app.MultiplayerManager.OnCheckAlignmentMarker();
            }

            for (var i = 0; i < 200; i++)
            {
                Refresh(app);
                if (app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked))
                    break;
                yield return new WaitForSecondsRealtime(.1f);
            }
            Require(app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked),
                "Both players did not finish spatial alignment.");
            for (var i = 0; i < 100 && duel.LocalMofaPlayer == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            Require(duel.LocalMofaPlayer != null, "Local MOFA player did not spawn.");
            duel.TryGetReady();
            for (var i = 0; i < 250 && duel.CurrentPhase.Value != MofaPhase.Fighting; i++)
                yield return new WaitForSecondsRealtime(.1f);
            Refresh(app);
            _report.fighting = duel.CurrentPhase.Value == MofaPhase.Fighting;
            Save();
            Require(_report.fighting, "Both players did not enter a Duel fighting round.");

            // Fighting starts before the first basic spell has finished charging.
            // A watch swing at that instant is correctly ignored by MofaInputManager.
            var input = FindObjectOfType<MofaInputManager>(true);
            Require(input != null, "Duel input manager did not initialize.");
            var selectedSchool = app.GlobalSettings.GetPreferencedObject();
            var basicSpell = duel.SpellList.List.FirstOrDefault(spell =>
                spell.SpellType == SpellType.Basic && spell.MagicSchool.TokenId == selectedSchool.TokenId);
            Require(basicSpell != null && basicSpell.MaxChargeCount > 0,
                "The selected basic spell or its charge capacity is missing.");
            _report.requiredBasicCharge = 1f / basicSpell.MaxChargeCount;
            for (var i = 0; i < 300; i++)
            {
                _report.lifeShieldReady = duel.LocalMofaPlayer.LifeShield != null &&
                                          !duel.LocalMofaPlayer.LifeShield.IsDestroyed;
                if (_report.lifeShieldReady && input.BasicSpellChargePercentage >= _report.requiredBasicCharge)
                    break;
                yield return new WaitForSecondsRealtime(.1f);
            }
            // The charge grows one physics step at a time and a swing casts only once it has reached a whole spell; at
            // 0.1999998 of 0.2 the swing is rightly ignored. Let one more step pass so the charge is past the line.
            yield return new WaitForFixedUpdate();
            _report.basicChargeBeforeShot = input.BasicSpellChargePercentage;
            Save();
            Require(_report.lifeShieldReady && _report.basicChargeBeforeShot >= _report.requiredBasicCharge,
                "Local life shield or basic spell charge was not ready for a Watch shot.");
            _report.localCastsBefore = duel.LocalMofaPlayer.CastCount.Value;
            MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
            _report.watchTriggers++;
            for (var i = 0; i < 100 && duel.LocalMofaPlayer.CastCount.Value <= _report.localCastsBefore; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.localCastsAfter = duel.LocalMofaPlayer.CastCount.Value;
            Require(_report.localCastsAfter > _report.localCastsBefore, "Simulated Watch shot did not cast locally.");
            yield return new WaitForSecondsRealtime(2);
            _report.remoteCastsSeen = duel.MofaPlayerDict.Values
                .Where(p => p != duel.LocalMofaPlayer).Sum(p => p.CastCount.Value);
            Capture("duel-fighting.png");
            Save();
            Require(_report.remoteCastsSeen > 0, "Remote player's spell count did not replicate.");
            Require(_report.errors.Count == 0, "Duel logged runtime errors: " + _report.errors.Count);
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
