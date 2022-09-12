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

        public Reality Reality;

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
        }

        public void InitializeNetworkManager()
        {
            if (Reality == null)
            {
                Debug.Log("[HoloKitApp] There is no reality selected");
                return;
            }

            if (Reality.realityManager == null)
            {
                Debug.Log($"[HoloKitApp] Reality {Reality.displayName} does not have a RealityManager");
            }

            RealityManager realityManager = Reality.realityManager.GetComponent<RealityManager>();
            if (realityManager == null)
            {
                Debug.Log($"[HoloKitApp] Reality {Reality.displayName} does not have a RealityManager script");
            }

            if (NetworkManager.Singleton == null)
            {
                var networkManager = Instantiate(NetworkManagerPrefab);
                networkManager.OnClientConnectedCallback += OnClientConnected;
                networkManager.AddNetworkPrefab(Reality.realityManager);
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

        public void EnterRealityAsHost()
        {
            _isHost = true;
            SceneManager.LoadScene(Reality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        public void JoinRealityAsSpectator()
        {
            _isHost = false;
            SceneManager.LoadScene(Reality.realityManager.GetComponent<RealityManager>().SceneName, LoadSceneMode.Single);
        }

        public void StartHost()
        {
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
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("[HoloKitApp] Client started");
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start client");
            }
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
            if (_isHost && clientId == NetworkManager.Singleton.LocalClientId)
            {
                var realityManager = Instantiate(Reality.realityManager.GetComponent<NetworkObject>());
                realityManager.Spawn();
            }
        }
    }
}