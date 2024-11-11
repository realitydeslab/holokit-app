// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

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
using RealityDesignLab.Library.Permissions;
using Holoi.Library.HoloKitAppLib.WatchConnectivity;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib
{
    [DisallowMultipleComponent]
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        /// <summary>
        /// This class is a singleton.
        /// </summary>
        private static HoloKitApp _instance;

        [Header("References")]
        /// <summary>
        /// Reference to the global settings scriptable object
        /// </summary>
        public HoloKitAppGlobalSettings GlobalSettings;

        /// <summary>
        /// Reference to the user account manager object
        /// </summary>
        [SerializeField] private HoloKitAppUserAccountManager _userAccountManager;

        [Header("Prefabs")]
        [SerializeField] private NetworkManager _networkManagerPrefab;

        [SerializeField] private HoloKitAppMultiplayerManager _multiplayerManagerPrefab;

        [SerializeField] private HoloKitAppARSessionManager _arSessionManagerPrefab;

        [SerializeField] private HoloKitAppRecorder _recorderPrefab;

        [Header("UI")]
        public UI.HoloKitAppUIPanelManager UIPanelManager;

        [SerializeField] private UniversalRenderPipelineAsset _urpAssetForUI;

        [Header("Version")]
        [SerializeField] private string _currentVersionNumber;

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
        public bool IsHost => _isHost;

        public HoloKitAppPlayerType LocalPlayerType => _localPlayerType;

        public bool IsPlayer => _localPlayerType == HoloKitAppPlayerType.Player;

        public bool IsSpectator => _localPlayerType == HoloKitAppPlayerType.Spectator;

        public bool IsPuppeteer => _localPlayerType == HoloKitAppPlayerType.Puppeteer;

        public int LocalPlayerTypeSubindex => _localPlayerTypeSubindex;

        public string CurrentVersionNumber => _currentVersionNumber;

        public HoloKitAppUserAccountManager UserAccountManager => _userAccountManager;

        public HoloKitAppMultiplayerManager MultiplayerManager => _multiplayerManager;

        public HoloKitAppARSessionManager ARSessionManager => _arSessionManager;

        public HoloKitAppRecorder Recorder => _recorder;

        public RealityManager RealityManager => _realityManager;

        private Reality _currentReality;

        private bool _isHost = true;

        private HoloKitAppPlayerType _localPlayerType = HoloKitAppPlayerType.Player;

        private int _localPlayerTypeSubindex = 0;

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

            Debug.unityLogger.logEnabled = _logEnabled;
            // Load Global Settings
            GlobalSettings.Load();
            // Register scene management delegates
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            // Push the initial UIPanel
            if (!IsRealityScene(SceneManager.GetActiveScene()))
            {
                UIPanelManager.PushUIPanel("LandingPage");
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
            if (_realityManager != null && _isHost)
                HoloKitAppAnalyticsEventManager.FireEvent_OnDreamOver(_realityManager.GetRealitySessionData());

            ResetWatchConnectivity();
            GlobalSettings.Save();
        }
        #endregion

        #region Reality Scene Management
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name.Equals("Start"))
            {
                UIPanelManager.OnStartSceneLoaded();
                SetUrpAssetForUI();
            } else if (scene.name.Equals("NoLiDAR")) {

            }
            else {
                InitializeRealityScene();
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (scene.name.Equals("NoLiDAR"))
            {
                DeinitializeARSession();
            } else if (scene.name.Equals("Start")) {

            } else {        
                DeinitializeRealityScene(); 
            }
        }

        /// <summary>
        /// This function checks whether a given scene is a Reality scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private bool IsRealityScene(Scene scene)
        {
            foreach (var reality in GlobalSettings.GetAllRealities())
            {
                if (reality.Scene == null) continue;

                if (reality.Scene.SceneName.Equals(scene.name))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Since this function is called within OnSceneLoaded(), this is called after
        /// OnEnable() and before Start().
        /// </summary>
        private void InitializeRealityScene()
        {
            // Find RealityManager reference
            _realityManager = FindFirstObjectByType<RealityManager>();
            if (_realityManager == null)
            {
                Debug.LogError("[HoloKitApp] There is no RealityManager in the scene");
                return;
            }
            _realityManager.FindConfig();
            if (_realityManager.Config == null)
            {
                Debug.LogError("[HoloKitApp] There is no RealityConfiguration script on the RealityManager");
                return;
            }
            // Setup URP Asset
            _realityManager.SetupURPAsset();

            // Spawn the ARSessionManager
            _arSessionManager = Instantiate(_arSessionManagerPrefab);
            // Spawn the Recorder
            _recorder = Instantiate(_recorderPrefab);
            // Spawn the MultiplayerManager
            _multiplayerManager = Instantiate(_multiplayerManagerPrefab);

            // Initialize NetworkManager
            InitializeNetworkManager();
            // We finally start the network after instantiating all necessary objects
            MultipeerConnectivityTransport.Instance.SessionId = _currentReality.BundleId;
            if (IsHost)
                StartHost();
            else
                StartClient();

            // Push Mono AR Panel as default
            UIPanelManager.PushUIPanel("MonoAR");
            // Push the reality specific UI Panel
            // TODO: Support more than 1 reality specific UI Panels
            if (_realityManager.Config.UIPanelPrefabs.Count > 0)
            {
                UIPanelManager.PushUIPanel(_realityManager.Config.UIPanelPrefabs[0]);
            }

            // TODO: Fix this
            // Set VideoEnhancementMode in HoloKitSDK
            if (GlobalSettings.HighResHDREnabled && _localPlayerType == HoloKitAppPlayerType.Spectator)
            {
                HoloKitCameraManager.Instance.VideoEnhancementMode = VideoEnhancementMode.HighResWithHDR;
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

        public void EnterRealityAs(bool isHost, HoloKitAppPlayerType playerType, int playerTypeSubindex = 0)
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

            _isHost = isHost;
            _localPlayerType = playerType;
            _localPlayerTypeSubindex = playerTypeSubindex;
            SceneManager.LoadScene(_currentReality.Scene.SceneName, LoadSceneMode.Single);
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
        /// <summary>
        /// This function initializes NetworkManager for a network session.
        /// It main functionality is to setup NetworkManager's NetworkPrefabsList
        /// and PlayerPrefab.
        /// </summary>
        private void InitializeNetworkManager()
        {
            // There can only one NetworkManager in the scene. Destroy it if there already one there.
            if (NetworkManager.Singleton != null)
                DestroyNetworkManager();

            // Instantiate the NetworkManager
            var networkManager = Instantiate(_networkManagerPrefab);
            // We use UnityTransport instead of MPC in eidtor mode for testing
            if (HoloKitUtils.IsEditor)
                networkManager.NetworkConfig.NetworkTransport = networkManager.GetComponent<UnityTransport>();
            else
                Destroy(networkManager.GetComponent<UnityTransport>());

            // Setup reality specific player prefab for NetworkManager
            if (_realityManager.Config.PlayerPrefab != null)
                networkManager.NetworkConfig.PlayerPrefab = _realityManager.Config.PlayerPrefab;
            // Setup reality specific network prefabs for NetworkManager
            foreach (var prefab in _realityManager.Config.NetworkPrefabs)
            {
                // Make sure all network prefabs contains a NetworkObject component
                if (prefab.TryGetComponent<NetworkObject>(out var _))
                    networkManager.AddNetworkPrefab(prefab);
                else
                    Debug.LogError($"[HoloKitApp] NetworkPrefab {prefab.name} does not have a NetworkObject component on it");
            }
        }

        private void DestroyNetworkManager()
        {
            if (NetworkManager.Singleton != null)
                Destroy(NetworkManager.Singleton.gameObject);
        }

        private void StartHost()
        {
            if (NetworkManager.Singleton == null)
                Debug.Log("[HoloKitApp] Failed to start host because NetworkManager is not initialized");

            if (NetworkManager.Singleton.StartHost())
                Debug.Log("[HoloKitApp] Host started");
            else
                Debug.Log("[HoloKitApp] Failed to start host");
        }

        private void StartClient()
        {
            if (NetworkManager.Singleton == null)
                Debug.Log("[HoloKitApp] Failed to start host because NetworkManager is not initialized");

            if (NetworkManager.Singleton.StartClient())
                Debug.Log("[HoloKitApp] Client started");
            else
                Debug.Log("[HoloKitApp] Failed to start client");
        }

        private void SetUrpAssetForUI()
        {
            if (_urpAssetForUI != null)
                GraphicsSettings.defaultRenderPipeline = _urpAssetForUI;
        }

        public void Shutdown()
        {
            if (_isHost)
                HoloKitAppAnalyticsEventManager.FireEvent_OnDreamOver(_realityManager.GetRealitySessionData());

            NetworkManager.Singleton.Shutdown();
            DestroyNetworkManager();
            SceneManager.LoadSceneAsync("Start", LoadSceneMode.Single);
        }
        #endregion
    }
}
