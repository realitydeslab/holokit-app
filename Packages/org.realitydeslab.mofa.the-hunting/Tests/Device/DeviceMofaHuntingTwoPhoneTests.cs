#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HoloKit.DeviceTests;
using Holoi.Library.HoloKitAppLib;
using MalbersAnimations.Controller;
using RealityDesignLab.MOFA.TheHunting.UI;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace RealityDesignLab.MOFA.TheHunting
{
    // Run on physical devices with MOFA_HUNTING_TWO_PHONE_ROLE=host (and MOFA_HUNTING_TWO_PHONE_CLIENTS=N) on the one
    // whose screen the others watch, and =client on the others; HOLOKIT_CLIENT_INDEX=2, 4, ... join as spectators.
    public sealed class DeviceMofaHuntingTwoPhoneTests : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public string device;
            public string role;
            public string playerType;
            public int expectedClients = 1;
            public bool complete;
            public bool arTracking;
            public bool networkListening;
            public int playerCount;
            public string localStatus;
            public bool connected;
            public bool spatialSynced;
            public bool checkMarkTapped;
            public bool spawnButtonTapped;
            public bool dragonSpawned;
            public bool portalSpawned;
            public bool dragonReady;
            public bool dragonFlying;
            public float dragonRise;
            public float dragonMoved;
            public string dragonStateWhileHeld, dragonState;
            public float networkRise, networkMoved;
            public string panelsAfterCheck;
            public List<string> errors = new();
        }

        // Metres the dragon must rise, and move when steered, to count as flying.
        private const float MinimumTravel = .05f;
        // Metres the host's updates must carry the dragon up and along on a client: the host's flight is far longer.
        private const float MinimumFlight = 1f;
        private const float ClientSettleSeconds = 5f;
        private const float ClientWatchSeconds = 25f;

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
            if (int.TryParse(Environment.GetEnvironmentVariable("MOFA_HUNTING_TWO_PHONE_CLIENTS"), out var clients))
                _report.expectedClients = clients;
            // Odd clients play, even ones watch as spectators.
            var spectator = int.TryParse(Environment.GetEnvironmentVariable("HOLOKIT_CLIENT_INDEX"), out var index) &&
                            index % 2 == 0;
            _report.playerType = (_report.role == "client" && spectator
                ? HoloKitAppPlayerType.Spectator : HoloKitAppPlayerType.Player).ToString();
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
            app.EnterRealityAs(_report.role == "host", Enum.Parse<HoloKitAppPlayerType>(_report.playerType));
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
                // The player taps the check button once the alignment marker sits on the host.
                for (var i = 0; i < 100 && !_report.checkMarkTapped; i++)
                {
                    yield return new WaitForSecondsRealtime(.1f);
                    _report.checkMarkTapped = DeviceTaps.TapCheckMark();
                }
                Require(_report.checkMarkTapped, "The alignment marker panel's check button did not appear or cannot " +
                                                 $"be tapped (covered by {DeviceTaps.LastBlocker}).");
                yield return new WaitForSecondsRealtime(2f);
                _report.panelsAfterCheck = DeviceTaps.PanelStack();
                var leftover = DeviceTaps.LeftoverJoinPanel();
                Require(leftover == null, $"The {leftover} panel still covers the reality after confirming the " +
                                          $"alignment (panels: {_report.panelsAfterCheck}).");
            }
            else
            {
                // The client may still be scanning the QR code: give it as long as its own scan loop above.
                for (var i = 0; i < 900 &&
                    !EveryoneChecked(app); i++)
                    yield return new WaitForSecondsRealtime(.1f);
                Require(EveryoneChecked(app),
                    "Client did not confirm the alignment marker.");
                Require(DeviceTaps.CloseShareQRCode(), "The QR code panel's close button is not shown.");
                var controller = FindObjectsOfType<MofaHuntingDragonControllerUIPanel>(true).FirstOrDefault();
                Require(controller != null, "Dragon controller panel is missing.");
                for (var i = 0; i < 50 && !_report.spawnButtonTapped; i++)
                {
                    yield return new WaitForSecondsRealtime(.1f);
                    _report.spawnButtonTapped = DeviceTaps.Tap(controller, nameof(controller.OnSpawnDragonButtonPressedFunc));
                }
                Require(_report.spawnButtonTapped, "Dragon summon button is not shown.");
            }
            for (var i = 0; i < 300 && (hunting.DragonController == null || hunting.PortalController == null); i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.dragonSpawned = hunting.DragonController != null;
            _report.portalSpawned = hunting.PortalController != null;
            Save();
            Capture("dragon-networked.png");
            Require(_report.dragonSpawned && _report.portalSpawned,
                "Dragon and portal did not replicate to both iPhones.");

            // Once the dragon has landed, the host flies it as its player does (holds the flying slider up, then
            // pushes the joystick forward) and the client watches it move.
            var dragon = hunting.DragonController;
            var animal = dragon.GetComponentInChildren<MAnimal>();
            Require(animal != null, "The dragon has no animal controller.");
            for (var i = 0; i < 300 && !dragon.ReadyForControl; i++)
                yield return new WaitForSecondsRealtime(.1f);
            _report.dragonReady = dragon.ReadyForControl;
            var start = animal.transform.position;
            if (_report.role == "host")
            {
                Require(_report.dragonReady, "The dragon never finished its entrance and became controllable.");
                // Every client sees the dragon land before it takes off, so each watches the whole flight.
                yield return new WaitForSecondsRealtime(ClientSettleSeconds);
                start = animal.transform.position;
                var controller = FindObjectsOfType<MofaHuntingDragonControllerUIPanel>(true).First();
                var slider = (Slider)typeof(MofaHuntingDragonControllerUIPanel)
                    .GetField("_flyingSlider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(controller);
                // A finger drags the slider up over several frames, and its onValueChanged wiring reaches the panel
                // each time the value changes; then the finger holds it at the top.
                controller.OnFlyingSliderPointerDown();
                for (var value = .55f; value <= 1f + 1e-4f; value += .05f)
                {
                    slider.value = Mathf.Min(value, 1f);
                    Watch(dragon, animal, start);
                    yield return null;
                }
                for (var until = Time.realtimeSinceStartup + 3f; Time.realtimeSinceStartup < until;)
                {
                    Watch(dragon, animal, start);
                    yield return null;
                }
                _report.dragonStateWhileHeld = animal.ActiveState != null ? animal.ActiveState.ID.name : "None";
                controller.OnFlyingSliderPointerUp();
                var joystick = controller.GetComponentsInChildren<MofaHuntingJoystickController>(true).First();
                var beforeSteering = animal.transform.position;
                for (var until = Time.realtimeSinceStartup + 3f; Time.realtimeSinceStartup < until;)
                {
                    joystick.OnAxisChanged.Invoke(Vector2.up); // What the joystick sends each frame while pushed forward.
                    yield return null;
                }
                joystick.OnAxisChanged.Invoke(Vector2.zero);
                _report.dragonMoved = Horizontal(animal.transform.position - beforeSteering);
                Capture("dragon-flying.png");
                yield return new WaitForSecondsRealtime(5f); // Let the client see the flight before the host stops.
            }
            else
            {
                // Where the host's latest transform update puts the dragon, next to where this device shows it: if the
                // first moves and the second does not, the updates arrive but something here overrides them.
                var networkTransform = dragon.GetComponent<NetworkTransform>();
                Require(networkTransform != null, "The dragon has no NetworkTransform.");
                var networkStart = networkTransform.GetSpaceRelativePosition(true);
                // The host's whole flight: it waits ClientSettleSeconds, climbs 3 s, steers 3 s.
                for (var until = Time.realtimeSinceStartup + ClientWatchSeconds; Time.realtimeSinceStartup < until;)
                {
                    Watch(dragon, animal, start);
                    _report.dragonMoved = Mathf.Max(_report.dragonMoved, Horizontal(animal.transform.position - start));
                    var network = networkTransform.GetSpaceRelativePosition(true) - networkStart;
                    _report.networkRise = Mathf.Max(_report.networkRise, network.y);
                    _report.networkMoved = Mathf.Max(_report.networkMoved, Horizontal(network));
                    yield return null;
                }
                Capture("dragon-flying.png");
                _report.dragonState = animal.ActiveState != null ? animal.ActiveState.ID.name : "None";
                Save();
                Require(_report.networkRise > MinimumFlight && _report.networkMoved > MinimumFlight,
                    $"The host's transform updates did not carry the flight here: they rose {_report.networkRise:F2} m " +
                    $"and moved {_report.networkMoved:F2} m.");
                Require(_report.dragonRise >= _report.networkRise / 2 && _report.dragonMoved >= _report.networkMoved / 2,
                    $"The dragon shown here did not follow the host's updates: it rose {_report.dragonRise:F2} m and " +
                    $"moved {_report.dragonMoved:F2} m; the updates rose {_report.networkRise:F2} m and moved " +
                    $"{_report.networkMoved:F2} m.");
            }
            _report.dragonState = animal.ActiveState != null ? animal.ActiveState.ID.name : "None";
            Save();
            Require(_report.dragonFlying, "The dragon did not take off.");
            Require(_report.dragonRise > MinimumTravel, $"The dragon rose only {_report.dragonRise:F2} m.");
            Require(_report.dragonMoved > MinimumTravel, $"The dragon moved only {_report.dragonMoved:F2} m when steered.");
            Require(_report.errors.Count == 0, "Hunting logged runtime errors: " + _report.errors.Count);
        }

        // Every client is connected and every player confirmed the alignment. Connected devices are counted, not
        // players: a client that has left drops out of the list, so the host alone would count as everyone.
        private bool EveryoneChecked(HoloKitApp app) =>
            NetworkManager.Singleton.ConnectedClientsIds.Count >= 1 + _report.expectedClients &&
            app.MultiplayerManager.PlayerList.All(p => p.PlayerStatus.Value == HoloKitAppPlayerStatus.Checked);

        private static float Horizontal(Vector3 offset) => new Vector2(offset.x, offset.z).magnitude;

        // Records whether the dragon has flown, and the highest it has risen above where it stood.
        private void Watch(DragonController dragon, MAnimal animal, Vector3 start)
        {
            _report.dragonFlying |= dragon.IsFlying;
            _report.dragonRise = Mathf.Max(_report.dragonRise, animal.transform.position.y - start.y);
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
