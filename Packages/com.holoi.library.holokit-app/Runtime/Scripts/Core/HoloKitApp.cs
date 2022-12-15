using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
        NonHostPlayer = 2,
        Puppeteer = 3
    }

    [DisallowMultipleComponent]
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        [Header("References")]
        public HoloKitAppGlobalSettings GlobalSettings;

        [SerializeField] private HoloKitAppUserAccountManager _userAccountManager;

        [Header("Prefabs")]
        [SerializeField] private NetworkManager _networkManagerPrefab;

        [SerializeField] private HoloKitAppMultiplayerManager _multiplayerManagerPrefab;

        [SerializeField] private HoloKitAppARSessionManager _arSessioinManagerPrefab;

        [SerializeField] private HoloKitAppRecorder _recorderPrefab;

        [Header("UI")]
        public UI.HoloKitAppUIPanelManager UIPanelManager;

        [SerializeField] private UniversalRenderPipelineAsset _urpAssetForUI;

        [Header("Debug")]
        [Tooltip("Set this to true to enable Test Mode. Under test mode, the TestRealityList will be loaded")]
        [SerializeField] private bool _testMode;

        [Tooltip("Set this to true to disable all Unity logs")]
        [SerializeField] private bool _logEnabled = true;

        [Tooltip("Set this value to true to restrict the fps to 60 in editor mode")]
        [SerializeField] private bool _lockFpsInEditor;

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
        public bool IsPlayer => _localPlayerType == HoloKitAppPlayerType.Host || _localPlayerType == HoloKitAppPlayerType.NonHostPlayer;

        public bool IsNonHostPlayer => _localPlayerType == HoloKitAppPlayerType.NonHostPlayer;

        public bool IsPuppeteer => _localPlayerType == HoloKitAppPlayerType.Puppeteer;

        public HoloKitAppPlayerType LocalPlayerType => _localPlayerType;

        public bool Test => _testMode;

        public HoloKitAppUserAccountManager UserAccountManager => _userAccountManager;

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

        public static event Action<string> OnEnteredReality; 

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

            Debug.unityLogger.logEnabled = _logEnabled;
            // Load Global Settings
            GlobalSettings.Load();
            // Register scene management delegates
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            // Push the initial UIPanel
            if (!IsRealityScene(SceneManager.GetActiveScene()))
            {
                // Push initial UI panel
                if (_testMode)
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
            // Activate WCSession
            HoloKitAppWatchConnectivityAPI.ActivateWatchConnectivitySession();
            // Set a default reality
            CurrentReality = GlobalSettings.RealityList.List[0];
            // Simulate iOS's FPS when testing on Editor
            if (HoloKitUtils.IsEditor && _lockFpsInEditor)
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
            else if (scene.name.Equals("Start"))
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
            else if (scene.name.Equals("NoLiDAR"))
            {
                DeinitializeARSession();
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
            // TODO: Add multiple layers of reality UI panels
            if (_realityManager.UIPanelPrefabs.Count > 0)
            {
                UIPanelManager.PushUIPanel(_realityManager.UIPanelPrefabs[0]);
            }

            // Set VideoEnhancementMode in HoloKitSDK
            if (GlobalSettings.HighResHDREnabled && _localPlayerType == HoloKitAppPlayerType.Spectator)
            {
                HoloKitCamera.Instance.VideoEnhancementMode = VideoEnhancementMode.HighResWithHDR;
            }
            return;
        }

        private void DeinitializeRealityScene()
        {
            if (HoloKitUtils.IsRuntime)
            {
                DeinitializeARSession();
                ResetWatchConnectivity();
            }
        }

        private void DeinitializeARSession()
        {
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.1f, () =>
            {
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
            }));
        }

        private void ResetWatchConnectivity()
        {
            // Make Watch App jump back to the main page
            HoloKitAppWatchConnectivityAPI.UpdateWatchPanel(HoloKitWatchPanel.None);
        }

        public void EnterRealityAs(HoloKitAppPlayerType playerType)
        {
            // Does the Reality we are going to enter need LiDAR?
            if (_currentReality.IsLiDARRequired())
            {
                if (!HoloKitOpticsAPI.IsCurrentDeviceEquippedWithLiDAR())
                //if (true)
                {
                    LoadNoLiDARScene();
                    return;
                }
            }

            _localPlayerType = playerType;
            SceneManager.LoadScene(_currentReality.Scene.SceneName, LoadSceneMode.Single);
            OnEnteredReality?.Invoke(_currentReality.BundleId);
        }

        private void LoadNoLiDARScene()
        {
            SceneManager.LoadScene("NoLiDAR", LoadSceneMode.Single);
            UIPanelManager.PushUIPanel("MonoAR_NoLiDAR");
        }

        public void ExitNoLiDARScene()
        {
            SceneManager.LoadScene("Start", LoadSceneMode.Single);
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
                networkManager.NetworkConfig.NetworkTransport = networkManager.GetComponent<UnityTransport>();
            }
            else
            {
                Destroy(networkManager.GetComponent<UnityTransport>());
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
            //Debug.Log("[HoloKitApp] NetworkManager initialized");
        }

        private void DeinitializeNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
                //Debug.Log("[HoloKitApp] NetworkManager deinitialized");
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
