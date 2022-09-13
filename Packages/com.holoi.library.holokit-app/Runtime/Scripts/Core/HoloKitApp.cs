using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Netcode.Transports.MultipeerConnectivity;
using System;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App
{
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        public NetworkManager NetworkManagerPrefab;

        [HideInInspector] public Reality CurrentReality;

        public bool IsHost => _isHost;

        public RealityManager RealityManager => _realityManager;

        private bool _isHost;

        private RealityManager _realityManager;

        public event Action OnConnectedAsHost;

        public event Action OnConnectedAsSpectator;

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
        }

        public void EnterRealityAsHost(Reality reality)
        {
            CurrentReality = reality;
            _isHost = true;
            SceneManager.LoadScene(CurrentReality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        public void JoinRealityAsSpectator(Reality reality)
        {
            CurrentReality = reality;
            _isHost = false;
            SceneManager.LoadScene(CurrentReality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        public void InitializeNetworkManager()
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

        public void DeinitializeNetworkManager()
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
            if (_realityManager != null)
            {
                Destroy(_realityManager.gameObject);
            }
            DeinitializeNetworkManager();
        }

        public void SetRealityManager(RealityManager realityManager)
        {
            _realityManager = realityManager;
        }

        public void StartAdvertising()
        {
            MultipeerConnectivityTransport.StartAdvertising();
        }

        public void StopAdvertising()
        {
            MultipeerConnectivityTransport.StopAdvertising();
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
                    OnConnectedAsHost?.Invoke();
                }
                else
                {
                    OnConnectedAsSpectator?.Invoke();
                }
            }
        }
    }
}