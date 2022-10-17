using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Holoi.AssetFoundation;
using Holoi.Library.Permissions;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        [Header("Prefabs")]
        [SerializeField] private NetworkManager _networkManagerPrefab;

        [SerializeField] private HoloKitAppMultiplayerManager _multiplayerManagerPrefab;

        [SerializeField] private HoloKitAppRecorder _recorderPrefab;

        [Header("Scriptable Objects")]
        public HoloKitAppGlobalSettings GlobalSettings;

        [Header("UI")]
        public UI.HoloKitAppUIPanelManager UIPanelManager;

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

        public bool IsHost => _isHost;

        public HoloKitAppMultiplayerManager MultiplayerManager => _multiplayerManager;

        public HoloKitAppRecorder Recorder => _recorder;

        public RealityManager RealityManager => _realityManager;

        private Reality _currentReality;

        private bool _isHost = true;

        private HoloKitAppMultiplayerManager _multiplayerManager;

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

            // Set screen orientation
            Screen.orientation = ScreenOrientation.Portrait;
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
            }
            StartCoroutine(HoloKitAppPermissionsManager.RequestWirelessDataPermission());
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
            CurrentReality = GlobalSettings.RealityList.realities[0];
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnApplicationQuit()
        {
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
                if (_isHost)
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
            // Pop AR UI Panels
            UIPanelManager.OnARSceneUnloaded();

            // Reset ARSession
            if (HoloKitUtils.IsRuntime)
            {
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
            }
        }

        public void EnterRealityAsHost()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] Failed to enter reality since CurrentReality is null");
                return;
            }
            _isHost = true;
            SceneManager.LoadScene(CurrentReality.Scene.SceneName, LoadSceneMode.Single);
        }

        public void JoinRealityAsSpectator()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] Failed to join reality since CurrentReality is null");
                return;
            }
            _isHost = false;
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
                Debug.Log("[HoloKitApp] Host started");

                var multiplayerManagerInstance = Instantiate(_multiplayerManagerPrefab);
                multiplayerManagerInstance.GetComponent<NetworkObject>().Spawn();

                _recorder = Instantiate(_recorderPrefab);
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
                Debug.Log("[HoloKitApp] Client started");

                _recorder = Instantiate(_recorderPrefab);
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start client");
            }
        }

        public void SetMultiplayerManager(HoloKitAppMultiplayerManager multiplayerManager)
        {
            _multiplayerManager = multiplayerManager;
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