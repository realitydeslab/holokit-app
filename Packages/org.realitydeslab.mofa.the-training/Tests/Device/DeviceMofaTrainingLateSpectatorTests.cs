#if DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// The spectator must also hear the round's background music, see the scores and the time left, must not see the
    /// distance labels over players' heads during the round, and must not record them: after the round, while waiting,
    /// it records and checks the labels.
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
            public bool hostMusicPlaying;
            public string fightingUtc;
            // Where each phone is in the music at a moment (UTC), to compare the host's and the spectator's afterwards.
            public float hostMusicTime;
            public string hostMusicUtc;
            public float musicTime;
            public string musicUtc;
            public string phase;
            public bool musicPlaying;
            public string musicClip;
            public float outputDb;
            public int distanceLabels;
            public bool scoresShown, timeShown;
            public string timeText;
            public string timeCheckedUtc;
            public bool waitingAgain;
            public int distanceLabelsWaiting;
            public bool recording;
            public int distanceLabelsRecording;
            public int distanceLabelsAfterRecording;
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
            _report.fightingUtc = DateTime.UtcNow.ToString("o");
            var ai = FindObjectsByType<MofaAIPlayer>(FindObjectsSortMode.None).FirstOrDefault();
            _report.avatarOnHost = ai != null && ai.AvatarAnimator != null;
            yield return new WaitForSecondsRealtime(1f);
            _report.hostMusicPlaying = MusicPlaying();
            var hostMusic = BackgroundMusic();
            _report.hostMusicTime = hostMusic != null ? hostMusic.time : -1f;
            _report.hostMusicUtc = DateTime.UtcNow.ToString("o");
            Save();
            Require(_report.fighting, "Training did not reach Fighting: " + manager.CurrentPhase.Value);
            Require(_report.avatarOnHost, "The host did not spawn the avatar.");
            // Only now advertise, as the spectator button does: the spectator joins a round already under way.
            app.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
            for (var i = 0; i < 1500 && !ClientChecked(app); i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.connected = ClientChecked(app);
            Save();
            Require(_report.connected, "The spectator did not join and confirm the alignment.");
            DeviceTaps.CloseShareQRCode();
            // The spectator checks the avatar and the music, then records once the round is over; stay until then.
            for (var i = 0; i < 1200 && manager.CurrentPhase.Value != MofaPhase.Waiting; i++)
                yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(25f);
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
            var manager = app.RealityManager as MofaBaseRealityManager;
            Require(manager != null, "The spectator has no MOFA reality manager.");
            _report.phase = manager.CurrentPhase.Value.ToString();
            var music = BackgroundMusic();
            _report.musicPlaying = music != null && music.isPlaying;
            _report.musicClip = music != null && music.clip != null ? music.clip.name : "";
            _report.musicTime = music != null ? music.time : -1f;
            _report.musicUtc = DateTime.UtcNow.ToString("o");
            var level = new OutputLevel();
            for (var i = 0; i < 20; i++)
            {
                level.Sample();
                yield return new WaitForSecondsRealtime(.05f);
            }
            _report.outputDb = level.Db;
            _report.distanceLabels = DistanceLabels();
            var scores = FindObjectsByType<MofaFightingPanelScores>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var times = FindObjectsByType<MofaFightingPanelTime>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _report.scoresShown = scores.Length > 0;
            _report.timeShown = times.Length > 0;
            _report.timeText = times.Length > 0 ? TimeText(times[0]) : "";
            _report.timeCheckedUtc = DateTime.UtcNow.ToString("o");
            Capture("spectator.png");
            Save();
            // After the round the labels come back while waiting; a recording must not show them.
            for (var i = 0; i < 1200 && manager.CurrentPhase.Value != MofaPhase.Waiting; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.waitingAgain = manager.CurrentPhase.Value == MofaPhase.Waiting;
            yield return new WaitForSecondsRealtime(2f);
            _report.distanceLabelsWaiting = DistanceLabels();
            Capture("spectator-waiting.png");
            app.Recorder.StartRecording();
            yield return new WaitForSecondsRealtime(2f);
            _report.recording = app.Recorder.IsRecording;
            _report.distanceLabelsRecording = DistanceLabels();
            Capture("spectator-recording.png");
            app.Recorder.StopRecording();
            yield return new WaitForSecondsRealtime(2f);
            _report.distanceLabelsAfterRecording = DistanceLabels();
            Save();
            Require(_report.aiPlayerSpawned, "The AI player never reached the spectator.");
            Require(_report.avatarBuilt, "The spectator joined after the round started and never built the AI avatar.");
            Require(_report.avatarRenderers > 0, "The spectator's AI avatar has nothing to render.");
            Require(_report.phase == nameof(MofaPhase.Fighting), "The spectator joined after the round: " + _report.phase);
            Require(_report.musicPlaying, "The spectator joined during the round but plays no background music.");
            Require(_report.scoresShown, "The spectator joined during the round but shows no scores.");
            Require(_report.timeShown, "The spectator joined during the round but shows no time.");
            var left = int.TryParse(_report.timeText.TrimEnd('s'), out var seconds) ? seconds : -1;
            Require(left > 0 && left < manager.RoundDuration - 1,
                $"The spectator shows {_report.timeText} for a round that started before it joined.");
            Require(_report.distanceLabels == 0,
                $"During the round the spectator shows {_report.distanceLabels} distance label(s) over players' heads.");
            Require(_report.waitingAgain, "The round did not end on the spectator.");
            Require(_report.distanceLabelsWaiting > 0, "No distance label while waiting, so the recording check proves nothing.");
            Require(_report.recording, "The spectator's recording did not start.");
            Require(_report.distanceLabelsRecording == 0,
                $"While recording the spectator shows {_report.distanceLabelsRecording} distance label(s).");
            Require(_report.distanceLabelsAfterRecording > 0, "The distance labels did not come back after the recording.");
        }

        // The AudioSource MOFA plays the round's background music on.
        private static AudioSource BackgroundMusic()
        {
            var effects = FindObjectOfType<MofaAudioEffectManager>(true);
            var field = typeof(MofaAudioEffectManager).GetField("_bgmAudioSource", BindingFlags.NonPublic | BindingFlags.Instance);
            return effects != null && field != null ? field.GetValue(effects) as AudioSource : null;
        }

        private static bool MusicPlaying()
        {
            var music = BackgroundMusic();
            return music != null && music.isPlaying;
        }

        // The text of the round's timer ("37s").
        private static string TimeText(MofaFightingPanelTime time)
        {
            var field = typeof(MofaFightingPanelTime).GetField("_gameTime", BindingFlags.NonPublic | BindingFlags.Instance);
            var text = field != null ? field.GetValue(time) : null;
            return text?.GetType().GetProperty("text")?.GetValue(text) as string ?? "";
        }

        private static int DistanceLabels() =>
            FindObjectsByType<HoloKitAppPlayerPoseVisualizer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;

        // The loudest block of what Unity sends to the speaker, in dBFS.
        private sealed class OutputLevel
        {
            private readonly float[] _samples = new float[1024];
            private float _peakRms;

            public void Sample()
            {
                AudioListener.GetOutputData(_samples, 0);
                var sum = 0f;
                foreach (var sample in _samples) sum += sample * sample;
                _peakRms = Mathf.Max(_peakRms, Mathf.Sqrt(sum / _samples.Length));
            }

            public float Db => _peakRms > 0f ? (float)Math.Round(20f * Mathf.Log10(_peakRms), 1) : -200f;
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
