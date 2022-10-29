using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Netcode.Transports.MultipeerConnectivity;
using Holoi.AssetFoundation;
using Holoi.Library.Permissions;
using Holoi.Library.HoloKitApp.WatchConnectivity;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public enum HoloKitAppPlayerType
    {
        Host = 0,
        Spectator = 1,
        NonHostPlayer = 2
    }

    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        [Header("Prefabs")]
        [SerializeField] private NetworkManager _networkManagerPrefab;

        [SerializeField] private HoloKitAppMultiplayerManager _multiplayerManagerPrefab;

        [SerializeField] private HoloKitAppARSessionManager _arSessioinManagerPrefab;

        [SerializeField] private HoloKitAppRecorder _recorderPrefab;

        [Header("Scriptable Objects")]
        public HoloKitAppGlobalSettings GlobalSettings;

        [Header("UI")]
        public UI.HoloKitAppUIPanelManager UIPanelManager;

        [SerializeField] private UniversalRenderPipelineAsset _urpAssetForUI;

        [Header("Debug")]
        // Set this to true to load TestRealityList at the beginning
        [SerializeField] private bool _test;

        public Reality CurrentReality
        {
            get => _currentReality;
            set
            {
                _currentReality = value;
            }
        }

        /// <summary>
        /// If the local device is the host
        /// </summary>
        public bool IsHost => _localPlayerType == HoloKitAppPlayerType.Host;

        /// <summary>
        /// If the local device is a spectator
        /// </summary>
        public bool IsSpectator => _localPlayerType == HoloKitAppPlayerType.Spectator;

        /// <summary>
        /// If the local player is the host or a non-host-player
        /// </summary>
        public bool IsPlayer => _localPlayerType != HoloKitAppPlayerType.Spectator;

        public bool IsNonHostPlayer => _localPlayerType == HoloKitAppPlayerType.NonHostPlayer;

        public HoloKitAppPlayerType LocalPlayerType => _localPlayerType;

        public bool Test => _test;

        public HoloKitAppMultiplayerManager MultiplayerManager => _multiplayerManager;

        public HoloKitAppARSessionManager ARSessionManager => _arSessionManager;

        public HoloKitAppRecorder Recorder => _recorder;

        public RealityManager RealityManager => _realityManager;

        private Reality _currentReality;

        private HoloKitAppPlayerType _localPlayerType = HoloKitAppPlayerType.Host;

        private HoloKitAppMultiplayerManager _multiplayerManager;

        private HoloKitAppARSessionManager _arSessionManager;

        private HoloKitAppRecorder _recorder;

        private RealityManager _realityManager;

        #region Mono
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);

            // Initialize HoloKit SDK
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitNFCSessionControllerAPI.RegisterNFCSessionControllerDelegates();
                HoloKitARSessionControllerAPI.RegisterARSessionControllerDelegates();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                HoloKitARSessionControllerAPI.SetSessionShouldAttemptRelocalization(false);
            }

            // Trigger WirelessData permission
            if (HoloKitUtils.IsRuntime)
            {
                PermissionsAPI.Initialize();
                StartCoroutine(HoloKitAppPermissionsManager.RequestWirelessDataPermission());
            }
            
            // Load Global Settings
            GlobalSettings.Load();
            // Register scene management delegates
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (!IsRealityScene(SceneManager.GetActiveScene()))
            {
                // Push initial UI panel
                if (_test)
                {
                    // Load test page
                    UIPanelManager.PushUIPanel("TestRealityList");
                }
                else
                {
                    // Load landing page
                    UIPanelManager.PushUIPanel("LandingPage");
                }
            }

            // Set a default reality
            CurrentReality = GlobalSettings.RealityList.List[0];

            // Activate WCSession
            HoloKitAppWatchConnectivityAPI.ActivateWatchConnectivitySession();

            // Simulate iOS's FPS when testing on Editor
            if (HoloKitUtils.IsEditor)
            {
                Application.targetFrameRate = 60;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnApplicationQuit()
        {
            ResetWatchConnectivity();
            GlobalSettings.Save();
        }
        #endregion

        #region Reality Scene Management
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (IsRealityScene(scene))
            {
                InitializeRealityScene();
            }
            else
            {
                UIPanelManager.OnStartSceneLoaded();
                SetUrpAssetForUI();
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (IsRealityScene(scene))
            {
                DeinitializeRealityScene();
            }
        }

        private bool IsRealityScene(Scene scene)
        {
            foreach (var reality in GlobalSettings.GetAllRealities())
            {
                if (reality.Scene == null) { continue; }

                if (reality.Scene.SceneName.Equals(scene.name))
                {
                    return true;
                }
            }
            return false;
            //return !scene.name.Equals("Start");
        }

        private void InitializeRealityScene()
        {
            // Find RealityManager reference
            _realityManager = FindObjectOfType<RealityManager>();
            if (_realityManager == null)
            {
                Debug.LogError("[HoloKitApp] There is no RealityManager in the scene");
                return;
            }
            // Setup URP Asset
            _realityManager.SetupURPAsset();

            // Initialize NetworkManager
            InitializeNetworkManager();

            // Wait and start network
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.5f, () =>
            {
                MultipeerConnectivityTransport.BundleId = _currentReality.BundleId;
                if (IsHost)
                    StartHost();
                else
                    StartClient();
            }));

            // Push AR UI Panel
            UIPanelManager.PushUIPanel("MonoAR");
            return;
        }

        private void DeinitializeRealityScene()
        {
            // Reset ARSession
            if (HoloKitUtils.IsRuntime)
            {
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();

                ResetWatchConnectivity();
            }
        }

        private void ResetWatchConnectivity()
        {
            // Let HoloKitAppWatchConnectivityManager take control of WCSessionDelegate
            HoloKitAppWatchConnectivityAPI.TakeControlWatchConnectivitySession();
            // Make Watch App jump back to the main page
            HoloKitAppWatchConnectivityAPI.UpdateCurrentReality(WatchReality.Nothing);
        }

        public void EnterRealityAs(HoloKitAppPlayerType playerType)
        {
            _localPlayerType = playerType;
            SceneManager.LoadScene(CurrentReality.Scene.SceneName, LoadSceneMode.Single);
        }
        #endregion

        #region Network Lifecycle
        private void InitializeNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                DeinitializeNetworkManager();
            }

            var networkManager = Instantiate(_networkManagerPrefab);
            if (HoloKitUtils.IsEditor)
            {
                networkManager.NetworkConfig.NetworkTransport = networkManager.GetComponent<UNetTransport>();
            }
            else
            {
                Destroy(networkManager.GetComponent<UNetTransport>());
            }
            foreach (var prefab in _realityManager.NetworkPrefabs)
            {
                if (prefab.TryGetComponent<NetworkObject>(out var _))
                {
                    networkManager.AddNetworkPrefab(prefab);
                }
                else
                {
                    Debug.Log($"[HoloKitApp] NetworkPrefab {prefab.name} does not have a NetworkObject component");
                }
            }
            Debug.Log("[HoloKitApp] NetworkManager initialized");
        }

        private void DeinitializeNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
                Debug.Log("[HoloKitApp] NetworkManager deinitialized");
            }
        }

        private void StartHost()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.Log("[HoloKitApp] Failed to start host because NetworkManager is not initialized");
            }

            if (NetworkManager.Singleton.StartHost())
            {
                SpawnMultiplayerManager();
                SpawnARSessionManager();
                SpawnRecorder();
                Debug.Log("[HoloKitApp] Host started");
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start host");
            }
        }

        private void StartClient()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.Log("[HoloKitApp] Failed to start host because NetworkManager is not initialized");
            }

            if (NetworkManager.Singleton.StartClient())
            {
                SpawnARSessionManager();
                SpawnRecorder();
                Debug.Log("[HoloKitApp] Client started");
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start client");
            }
        }

        private void SpawnMultiplayerManager()
        {
            var multiplayerManagerInstance = Instantiate(_multiplayerManagerPrefab);
            multiplayerManagerInstance.GetComponent<NetworkObject>().Spawn();
        }

        private void SpawnARSessionManager()
        {
            _arSessionManager = Instantiate(_arSessioinManagerPrefab);
        }

        private void SpawnRecorder()
        {
            _recorder = Instantiate(_recorderPrefab);
        }

        public void SetMultiplayerManager(HoloKitAppMultiplayerManager multiplayerManager)
        {
            _multiplayerManager = multiplayerManager;
        }

        private void SetUrpAssetForUI()
        {
            if (_urpAssetForUI != null)
            {
                GraphicsSettings.renderPipelineAsset = _urpAssetForUI;
            }
        }

        public void Shutdown()
        {
            NetworkManager.Singleton.Shutdown();
            DeinitializeNetworkManager();

            SceneManager.LoadSceneAsync("Start", LoadSceneMode.Single);
        }
        #endregion
    }
}