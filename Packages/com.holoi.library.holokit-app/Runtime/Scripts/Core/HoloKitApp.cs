using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Holoi.HoloKit.App
{
    public class HoloKitApp : MonoBehaviour
    {
        public static HoloKitApp Instance { get { return _instance; } }

        private static HoloKitApp _instance;

        [SerializeField] private NetworkManager _networkManagerPrefab;

        [SerializeField] private GameObject[] _networkPrefabs;

        [SerializeField] private RealityManager _realityManagerPrefab;

        public RealityManager RealityManager => _realityManager;

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

        private void InitializeNetworkManager()
        {
            if (NetworkManager.Singleton == null)
            {
                Instantiate(_networkManagerPrefab);
                foreach (var networkPrefab in _networkPrefabs)
                {
                    NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
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

        public void EnterRealityAsHost()
        {
            InitializeNetworkManager();
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("[HoloKitApp] Host started");
                var realityManager = Instantiate(_realityManagerPrefab);
                realityManager.GetComponent<NetworkObject>().Spawn();
                NetworkManager.Singleton.SceneManager.LoadScene("SampleRealityAR", LoadSceneMode.Single);
            }
            else
            {
                Debug.Log("[HoloKitApp] Failed to start host");
            }
        }

        public void JoinRealityAsSpectator()
        {
            InitializeNetworkManager();
        }

        public void SetRealityManager(RealityManager realityManager)
        {
            _realityManager = realityManager;
        }
    }
}