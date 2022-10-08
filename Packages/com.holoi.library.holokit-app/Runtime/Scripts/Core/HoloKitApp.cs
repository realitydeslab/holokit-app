using System.Collections;
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

        private bool _isHost;

        private HoloKitAppMultiplayerManager _multiplayerManager;

        private HoloKitAppRecorder _recorder;

        private RealityManager _realityManager;

        #region Mono
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);

            // Set screen orientation
            Screen.orientation = ScreenOrientation.Portrait;
            // Initialize HoloKit SDK
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitNFCSessionControllerAPI.RegisterNFCSessionControllerDelegates();
                HoloKitARSessionControllerAPI.RegisterARSessionControllerDelegates();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                HoloKitARSessionControllerAPI.SetSessionShouldAttemptRelocalization(false);
            }
            // Trigger WirelessData permission
            if (HoloKitHelper.IsRuntime)
            {
                PermissionsAPI.Initialize();
            }
            StartCoroutine(HoloKitAppPermissionsManager.RequestWirelessDataPermission());
            // Load Global Settings
            GlobalSettings.Load();
            // Register scene management delegates
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            // Push UI panel if not in a reality scene
            if (IsRealityScene(SceneManager.GetActiveScene()))
            {
                // Now it is in debug mode
                _isHost = true;
            }
            else
            {
                // Push initial UI panel
                if (_test)
                {
                    UI.HoloKitAppUIPanelManager.Instance.PushUIPanel("TestRealityList");
                }
                else
                {
                    // TODO: Load main UI panel
                }
            }
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
                if (reality.scene == null) { continue; }

                if (reality.scene.SceneName.Equals(scene.name))
                {
                    return true;
                }
            }
            return false;
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

            // Wait and start network
            StartCoroutine(StartNetworkWithDelay(0.5f));

            // Push AR UI Panel
            UI.HoloKitAppUIPanelManager.Instance.PushUIPanel("MonoAR");
            return;
        }

        private void DeinitializeRealityScene()
        {
            // Pop AR UI Panels
            UI.HoloKitAppUIPanelManager.Instance.OnARSceneUnloaded();

            // Reset ARSession
            if (HoloKitHelper.IsRuntime)
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
            SceneManager.LoadScene(CurrentReality.scene.SceneName, LoadSceneMode.Single);
        }

        public void JoinRealityAsSpectator()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] Failed to join reality since CurrentReality is null");
                return;
            }
            _isHost = false;
            SceneManager.LoadScene(CurrentReality.scene.SceneName, LoadSceneMode.Single);
        }
        #endregion

        #region Network Lifecycle
        private IEnumerator StartNetworkWithDelay(float t)
        {
            yield return new WaitForSeconds(t);
            InitializeNetworkManager();
            if (_isHost)
            {
                StartHost();
            }
            else
            {
                StartClient();
            }
        }

        private void InitializeNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                DeinitializeNetworkManager();
            }

            var networkManager = Instantiate(_networkManagerPrefab);
            if (HoloKitHelper.IsEditor)
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