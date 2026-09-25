#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloKit;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.Library.Base.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheTraining
{
    /// <summary>
    /// Opt-in physical-iPhone combat check. Taps the game's Star control, enters
    /// Training combat, and injects the event normally emitted after a watch swing.
    /// </summary>
    public sealed class DeviceMofaCombatTests : MonoBehaviour
    {
        [Serializable]
        private sealed class CameraView
        {
            public string camera;
            public Vector3 avatarViewport;
            public bool insideViewport;
        }

        [Serializable]
        private sealed class Report
        {
            public string device;
            public string product;
            public bool complete;
            public bool arTracking;
            public bool hostListening;
            public bool aiSpawned;
            public bool aiAvatarVisible;
            public bool aiWeaponPresent;
            public bool stereoRequested;
            public string renderMode;
            public bool trackingDeviceFound;
            public float centerEyeY;
            public float placementY;
            public float aiOriginY;
            public float aiDistanceFromEye;
            public int aiBodyRenderers;
            public Vector3 aiBodyBoundsCenter;
            public Vector3 aiBodyBoundsSize;
            public List<CameraView> aiCameraViews = new();
            public int aiFallbackMaterials;
            public int aiCasts;
            public bool starButtonActive;
            public bool syntheticPlacement;
            public bool fighting;
            public int watchTriggers;
            public int castsBefore;
            public int castsAfter;
            public List<string> screenshots = new();
            public List<string> errors = new();
        }

        private readonly Report _report = new();
        private string _directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartRequestedRun()
        {
            if (!Environment.GetCommandLineArgs().Contains("--mofa-combat-test") &&
                Environment.GetEnvironmentVariable("MOFA_COMBAT_TEST") != "1") return;
            Debug.Log("[MOFA COMBAT TEST] Starting physical-device fight and simulated watch trigger.");
            var runner = new GameObject("MOFA Device Combat Tests");
            DontDestroyOnLoad(runner);
            runner.AddComponent<DeviceMofaCombatTests>();
        }

        private IEnumerator Start()
        {
            var previousSleepTimeout = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.logMessageReceived += OnLog;
            _directory = Path.Combine(Application.persistentDataPath, "MofaCombatTests");
            Directory.CreateDirectory(_directory);
            _report.device = SystemInfo.deviceModel;
            _report.product = Application.identifier;
            Save();
            var run = Run();
            while (true)
            {
                bool more;
                object next;
                try { more = run.MoveNext(); next = more ? run.Current : null; }
                catch (Exception exception)
                {
                    _report.errors.Add(exception.ToString());
                    break;
                }
                if (!more) { _report.complete = true; break; }
                yield return next;
            }
            Save();
            Application.logMessageReceived -= OnLog;
            Screen.sleepTimeout = previousSleepTimeout;
            Debug.Log($"[MOFA COMBAT TEST] COMPLETE={_report.complete} fighting={_report.fighting} casts={_report.castsBefore}->{_report.castsAfter} errors={_report.errors.Count} report={_directory}/results.json");
            Destroy(gameObject);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(3);
            var app = HoloKitApp.Instance;
            Require(app != null, "HoloKitApp did not start.");
            _report.stereoRequested = Environment.GetEnvironmentVariable("MOFA_COMBAT_STEREO_TEST") == "1";
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Require(Application.HasUserAuthorization(UserAuthorization.WebCam), "Camera permission is required.");
            var training = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == "MOFATheTraining");
            Require(training != null, "MOFA Training is missing from the product catalog.");
            app.CurrentReality = training;
            app.EnterRealityAs(true, HoloKitAppPlayerType.Player);
            yield return new WaitForSecondsRealtime(6);
            Require(SceneManager.GetActiveScene().name == "MOFATheTraining", "Training scene did not open.");
            var manager = app.RealityManager as MofaTrainingRealityManager;
            Require(manager != null, "Training reality manager did not initialize.");
            Require(HoloKitCameraManager.Instance != null, "Training camera rig did not initialize.");
            if (_report.stereoRequested)
            {
                app.UIPanelManager.PushUIPanel("StarAR", HoloKitAppUICanvasType.StAR);
                HoloKitCameraManager.Instance.OpenStereoWithoutNFC("SomethingForNothing");
                yield return new WaitForSecondsRealtime(2);
            }
            _report.renderMode = HoloKitCameraManager.Instance.RenderMode.ToString();
            _report.trackingDeviceFound = FindObjectOfType<HoloKitTrackedPoseDriver>(true)?.HasTrackingDevice == true;
            Save();
            if (_report.stereoRequested)
                Require(_report.renderMode == HoloKitRenderMode.Stereo.ToString() && _report.trackingDeviceFound,
                    "Stereo mode or its ARKit tracking device did not initialize.");
            for (var i = 0; i < 80 && manager.LocalMofaPlayer == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            Require(manager.LocalMofaPlayer != null, "Local MOFA player did not spawn.");
            for (var i = 0; i < 300 && ARSession.state != ARSessionState.SessionTracking; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.arTracking = ARSession.state == ARSessionState.SessionTracking;
            _report.hostListening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsListening;
            _report.aiSpawned = FindObjectOfType<MofaAIPlayer>(true) != null;
            Save();
            Require(_report.arTracking && _report.hostListening && _report.aiSpawned, "AR, host, or AI opponent is not ready.");

            var placement = FindObjectOfType<ARPlacementManager>(true);
            Require(placement != null, "Training placement manager is missing.");
            for (var i = 0; i < 80 && !placement.IsValid; i++)
                yield return new WaitForSecondsRealtime(.1f);
            if (!placement.IsValid)
            {
                // Keep the real-device combat test runnable if the current room
                // offers no horizontal AR plane. The report records this fallback.
                placement.IsActive = false;
                // ARPlacementManager has already updated this configured default
                // point; only make it selectable for the synthetic combat run.
                placement.HitPoint.gameObject.SetActive(true);
                _report.syntheticPlacement = true;
            }
            _report.centerEyeY = HoloKitCameraManager.Instance.CenterEyePose.position.y;
            _report.placementY = placement.HitPoint.position.y;
            Save();

            if (_report.stereoRequested)
            {
                var stereoButton = FindObjectsOfType<HoloKitAppUIComponent_StarAR_TriggerButton>(true)
                    .FirstOrDefault(item => item.isActiveAndEnabled);
                _report.starButtonActive = stereoButton != null;
                Require(stereoButton != null, "The visible stereo trigger button is missing.");
                Debug.Log("[MOFA COMBAT TEST] Tapping the stereo trigger button to start the round.");
                stereoButton.OnPointerDown(null);
                stereoButton.OnPointerUp(null);
            }
            else
            {
                var button = FindObjectsOfType<MofaUIPanel>(true)
                    .FirstOrDefault(item => item.isActiveAndEnabled);
                _report.starButtonActive = button != null;
                Require(button != null, "The visible MOFA start panel is missing.");
                Debug.Log("[MOFA COMBAT TEST] Tapping the MOFA start button to start the round.");
                button.OnTriggerButtonPressed();
            }
            for (var i = 0; i < 120 && manager.CurrentPhase.Value != MofaPhase.Fighting; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.fighting = manager.CurrentPhase.Value == MofaPhase.Fighting;
            Save();
            Require(_report.fighting, "The Star tap did not advance Training to Fighting: " + manager.CurrentPhase.Value);
            Debug.Log("[MOFA COMBAT TEST] Fighting phase entered.");
            var ai = FindObjectOfType<MofaAIPlayer>(true);
            for (var i = 0; i < 80 && ai.AvatarAnimator == null; i++)
                yield return new WaitForSecondsRealtime(.1f);
            for (var i = 0; i < 80 && !ai.AvatarWeaponPresent; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.aiAvatarVisible = ai.AvatarAnimator != null && ai.AvatarAnimator.gameObject.activeInHierarchy;
            _report.aiOriginY = ai.transform.position.y;
            _report.aiWeaponPresent = ai.AvatarWeaponPresent;
            _report.aiFallbackMaterials = ai.AvatarFallbackMaterialCount;
            RecordAiCameraViews(ai);
            Save();
            Require(_report.aiAvatarVisible, "AI opponent avatar did not appear.");
            Require(_report.aiWeaponPresent, "AI opponent wand did not spawn on the hand joint.");
            Require(_report.aiFallbackMaterials > 0, "AI avatar did not replace the incompatible Meebits shader on iOS.");
            yield return new WaitForEndOfFrame();
            Capture("fighting-before-shot.png");

            var input = FindObjectOfType<MofaInputManager>(true);
            Require(input != null, "MOFA input manager is missing.");
            _report.castsBefore = manager.LocalMofaPlayer.CastCount.Value;
            // A wrist swing is recognized on the watch when abs(x acceleration)
            // crosses its threshold. Inject the resulting WatchConnectivity event;
            // the Watch's CoreMotion sensor and classifier are outside this test.
            yield return new WaitForSecondsRealtime(5);
            MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
            _report.watchTriggers++;
            Debug.Log("[MOFA COMBAT TEST] Injected simulated Apple Watch shooting event.");
            yield return new WaitForSecondsRealtime(.15f);
            yield return new WaitForEndOfFrame();
            Capture("watch-shot-1.png");
            for (var i = 0; i < 20 && manager.LocalMofaPlayer.CastCount.Value <= _report.castsBefore; i++)
                yield return new WaitForSecondsRealtime(.1f);
            if (manager.LocalMofaPlayer.CastCount.Value <= _report.castsBefore)
            {
                yield return new WaitForSecondsRealtime(5);
                MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                _report.watchTriggers++;
                yield return new WaitForSecondsRealtime(.15f);
                yield return new WaitForEndOfFrame();
                Capture("watch-shot-2.png");
            }
            _report.castsAfter = manager.LocalMofaPlayer.CastCount.Value;
            Require(_report.castsAfter > _report.castsBefore, "Simulated watch shooting did not spawn a spell.");
            Debug.Log($"[MOFA COMBAT TEST] Spell cast recorded: {_report.castsBefore}->{_report.castsAfter}.");
            for (var i = 0; i < 200 && ai.CastCount.Value == 0; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.aiCasts = ai.CastCount.Value;
            RecordAiCameraViews(ai);
            Save();
            Capture("ai-attack.png");
            Require(_report.aiCasts > 0, "AI opponent did not cast a spell during combat.");
            Require(_report.errors.Count == 0, "Combat produced runtime errors: " + _report.errors.Count);
        }

        private void RecordAiCameraViews(MofaAIPlayer ai)
        {
            var renderers = ai.AvatarAnimator.GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .Where(renderer => renderer.gameObject.activeInHierarchy && renderer.enabled).ToArray();
            _report.aiBodyRenderers = renderers.Length;
            _report.aiCameraViews.Clear();
            if (renderers.Length == 0) return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            _report.aiBodyBoundsCenter = bounds.center;
            _report.aiBodyBoundsSize = bounds.size;
            _report.aiDistanceFromEye = Vector3.Distance(HoloKitCameraManager.Instance.CenterEyePose.position, bounds.center);
            foreach (var camera in Camera.allCameras)
            {
                var viewport = camera.WorldToViewportPoint(bounds.center);
                _report.aiCameraViews.Add(new CameraView
                {
                    camera = camera.name,
                    avatarViewport = viewport,
                    insideViewport = viewport.z > 0f && viewport.x >= 0f && viewport.x <= 1f &&
                                     viewport.y >= 0f && viewport.y <= 1f
                });
            }
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
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                _report.errors.Add(message + "\n" + stackTrace);
        }

        private void Save() => File.WriteAllText(Path.Combine(_directory, "results.json"), JsonUtility.ToJson(_report, true));
    }
}
#endif
