#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit.DeviceTests;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.Library.Base.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealityDesignLab.MOFA.TheTraining
{
    /// <summary>
    /// A spectator who joins MOFA Training after the host has started it must still see the AI avatar. The host starts
    /// the first round (which spawns the avatar) before it shows its QR code, so the spectator can only join afterwards;
    /// the spectator then scans the code, taps the check button and must have built the avatar.
    /// Run with MOFA_TRAINING_LATE_SPECTATOR_ROLE=host on the device whose screen the other watches, =client on it.
    /// </summary>
    public sealed class DeviceMofaTrainingLateSpectatorTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public string device;
            public string role;
            public bool complete;
            public bool fighting;
            public bool avatarOnHost;
            public bool connected, synced, checkMarkTapped;
            public bool aiPlayerSpawned, avatarBuilt;
            public int avatarRenderers;
            public float avatarSeconds;
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            var role = Environment.GetEnvironmentVariable("MOFA_TRAINING_LATE_SPECTATOR_ROLE");
            if (role != "host" && role != "client") return;
            var runner = new GameObject("MOFA Training Late Spectator Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaTrainingLateSpectatorTests>()._report.role = role;
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _directory = Path.Combine(Application.persistentDataPath, "MofaTrainingLateSpectatorTests");
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
            Debug.Log($"[MOFA TRAINING LATE SPECTATOR] role={_report.role} complete={_report.complete} " +
                      $"avatarBuilt={_report.avatarBuilt} errors={_report.errors.Count}");
        }

        private IEnumerator Run()
        {
            for (var i = 0; i < 100 && HoloKitApp.Instance == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            yield return new WaitForSecondsRealtime(2f);
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheTraining");
            Require(reality != null, "Training is missing from the catalog.");
            app.CurrentReality = reality;
            var host = _report.role == "host";
            app.EnterRealityAs(host, host ? HoloKitAppPlayerType.Player : HoloKitAppPlayerType.Spectator);
            for (var i = 0; i < 100 && SceneManager.GetActiveScene().name != "MOFATheTraining"; i++)
                yield return new WaitForSecondsRealtime(.2f);
            Require(SceneManager.GetActiveScene().name == "MOFATheTraining", "Training did not open.");
            var steps = host ? Host(app) : Client(app);
            while (steps.MoveNext())
                yield return steps.Current;
        }

        private IEnumerator Host(HoloKitApp app)
        {
            yield return new WaitForSecondsRealtime(3f);
            var manager = app.RealityManager as MofaTrainingRealityManager;
            Require(manager != null, "Training reality manager did not initialize.");
            // Start the round as the player does: aim at the floor, press the trigger. This spawns the avatar.
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
            var ai = FindObjectsByType<MofaAIPlayer>(FindObjectsSortMode.None).FirstOrDefault();
            _report.avatarOnHost = ai != null && ai.AvatarAnimator != null;
            Save();
            Require(_report.fighting, "Training did not reach Fighting: " + manager.CurrentPhase.Value);
            Require(_report.avatarOnHost, "The host did not spawn the avatar.");
            // Only now advertise, as the spectator button does: the spectator joins a round already under way.
            app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
            for (var i = 0; i < 1500 && !ClientChecked(app); i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.connected = ClientChecked(app);
            Require(_report.connected, "The spectator did not join and confirm the alignment.");
            DeviceTaps.CloseShareQRCode();
            yield return new WaitForSecondsRealtime(15f); // The spectator checks the avatar before the host leaves.
        }

        private IEnumerator Client(HoloKitApp app)
        {
            for (var i = 0; i < 1500 && !Synced(app); i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.connected = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
            _report.synced = Synced(app);
            Require(_report.synced, "The spectator did not align with the host's QR code.");
            for (var i = 0; i < 100 && !_report.checkMarkTapped; i++)
            {
                yield return new WaitForSecondsRealtime(.1f);
                _report.checkMarkTapped = DeviceTaps.TapCheckMark();
            }
            Require(_report.checkMarkTapped, "The check button did not appear or cannot be tapped.");
            var started = Time.realtimeSinceStartup;
            MofaAIPlayer ai = null;
            for (var i = 0; i < 100 && (ai == null || ai.AvatarAnimator == null); i++)
            {
                ai = FindObjectsByType<MofaAIPlayer>(FindObjectsSortMode.None).FirstOrDefault();
                yield return new WaitForSecondsRealtime(.1f);
            }
            _report.avatarSeconds = (float)Math.Round(Time.realtimeSinceStartup - started, 1);
            _report.aiPlayerSpawned = ai != null;
            _report.avatarBuilt = ai != null && ai.AvatarAnimator != null;
            _report.avatarRenderers = ai == null ? 0 : ai.GetComponentsInChildren<Renderer>()
                .Count(r => r.enabled && r.gameObject.activeInHierarchy);
            yield return new WaitForSecondsRealtime(2f);
            Capture("spectator.png");
            Save();
            Require(_report.aiPlayerSpawned, "The AI player never reached the spectator.");
            Require(_report.avatarBuilt, "The spectator joined after the round started and never built the AI avatar.");
            Require(_report.avatarRenderers > 0, "The spectator's AI avatar has nothing to render.");
        }

        // The spectator is connected and every player, the AI one included, confirmed the alignment.
        private static bool ClientChecked(HoloKitApp app) =>
            NetworkManager.Singleton != null && NetworkManager.Singleton.ConnectedClientsIds.Count >= 2 &&
            app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked);

        private static bool Synced(HoloKitApp app)
        {
            var local = app.MultiplayerManager != null ? app.MultiplayerManager.LocalPlayer : null;
            return local != null && (local.PlayerStatus.Value == HoloKitAppPlayerStatus.Synced ||
                                     local.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked);
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

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
