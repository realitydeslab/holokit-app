using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Holoi.AssetFoundation;
using Unity.Netcode.Transports.UNET;
using HoloKit;
using Holoi.Library.Permissions;
using UnityEngine.XR.ARFoundation;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        public NetworkManager NetworkManagerPrefab;

        public HoloKitAppLocalPlayerPreferences LocalPlayerPreferences;

        public GameObject PhoneAlignmentMarkPrefab;

        [HideInInspector] public Reality CurrentReality;

        public bool IsHost => _isHost;

        public RealityManager RealityManager => _realityManager;

        private bool _isHost;

        private RealityManager _realityManager;

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

            // Initialize HoloKit SDK
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitNFCSessionControllerAPI.RegisterNFCSessionControllerDelegates();
                HoloKitARSessionControllerAPI.RegisterARSessionControllerDelegates();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                HoloKitARSessionControllerAPI.SetSessionShouldAttemptRelocalization(false);
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (var reality in LocalPlayerPreferences.RealityList.realities)
            {
                if (reality.realityManager == null)
                {
                    continue;
                }

                if (reality.realityManager.GetComponent<RealityManager>().SceneName.Equals(scene.name))
                {
                    UI.HoloKitAppUIPanelManager.Instance.PushUIPanel("MonoAR");

                    // Wait and start network
                    StartCoroutine(StartNetworkWithDelay(0.5f));

                    return;
                }
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            foreach (var reality in LocalPlayerPreferences.RealityList.realities)
            {
                if (reality.realityManager == null)
                {
                    continue;
                }

                if (reality.realityManager.GetComponent<RealityManager>().SceneName.Equals(scene.name))
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
                    return;
                }
            }
        }

        private IEnumerator StartNetworkWithDelay(float t)
        {
            yield return new WaitForSeconds(t);
            if (_isHost)
            {
                StartHost();
            }
            else
            {
                StartClient();
            }
        }

        private void Start()
        {
            LocalPlayerPreferences.Load();
            Screen.orientation = ScreenOrientation.Portrait;
            if (HoloKitHelper.IsRuntime)
            {
                PermissionsAPI.Initialize();
            }
            StartCoroutine(HoloKitAppPermissionsManager.RequestWirelessDataPermission());
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnApplicationQuit()
        {
            LocalPlayerPreferences.Save();
        }

        public void EnterRealityAsHost()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] Failed to enter reality since CurrentReality is null");
                return;
            }
            _isHost = true;
            SceneManager.LoadScene(CurrentReality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        public void JoinRealityAsSpectator()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] Failed to join reality since CurrentReality is null");
                return;
            }
            _isHost = false;
            SceneManager.LoadScene(CurrentReality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        private void InitializeNetworkManager()
        {
            if (CurrentReality == null)
            {
                Debug.Log("[HoloKitApp] There is no reality selected");
                return;
            }

            if (CurrentReality.realityManager == null)
            {
                Debug.Log($"[HoloKitApp] Reality {CurrentReality.displayName} does not have a RealityManager");
            }

            RealityManager realityManager = CurrentReality.realityManager.GetComponent<RealityManager>();
            if (realityManager == null)
            {
                Debug.Log($"[HoloKitApp] Reality {CurrentReality.displayName} does not have a RealityManager script");
            }

            if (NetworkManager.Singleton == null)
            {
                var networkManager = Instantiate(NetworkManagerPrefab);
                if (HoloKitHelper.IsEditor)
                {
                    networkManager.NetworkConfig.NetworkTransport = networkManager.GetComponent<UNetTransport>();
                }
                networkManager.OnClientConnectedCallback += OnClientConnected;
                networkManager.AddNetworkPrefab(CurrentReality.realityManager);
                foreach (var prefab in realityManager.NetworkPrefabs)
                {
                    if (prefab.TryGetComponent<NetworkObject>(out var _))
                    {
                        networkManager.AddNetworkPrefab(prefab);
                    }
                }
                Debug.Log("[HoloKitApp] NetworkManager initialized");
            }
            else
            {
                Debug.Log("[HoloKitApp] NetworkManager already initialized");
            }
        }

        private void DeinitializeNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
                Debug.Log("[HoloKitApp] NetworkManager deinitialized");
            }
            else
            {
                Debug.Log("[HoloKitApp] There is no NetworkManager to destroy");
            }
        }

        public void StartHost()
        {
            InitializeNetworkManager();

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("[HoloKitApp] Host started");
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start host");
            }
        }

        public void StartClient()
        {
            InitializeNetworkManager();

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("[HoloKitApp] Client started");
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start client");
            }
        }

        public void Shutdown()
        {
            NetworkManager.Singleton.Shutdown();
            DeinitializeNetworkManager();

            SceneManager.LoadSceneAsync(UI.HoloKitAppUIPanelManager.Instance.InitialScene, LoadSceneMode.Single);
        }

        public void SetRealityManager(RealityManager realityManager)
        {
            _realityManager = realityManager;
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"[HoloKitApp] OnClientConnected {clientId}");
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                if (_isHost)
                {
                    var realityManager = Instantiate(CurrentReality.realityManager.GetComponent<NetworkObject>());
                    realityManager.Spawn();
                }
            }
        }
    }
}