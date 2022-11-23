using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Reality.BallAndChain
{
    public class BallAndChainRealityManager : RealityManager
    {
        [SerializeField] private GameObject _chain;

        [SerializeField] private NetworkObject _dynamicallySpawnedBallPrefab;

        private void Awake()
        {
            HoloKitAppMultiplayerManager.OnAlignmentMarkerChecked += OnRelocalizationSucceeded;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            HoloKitAppMultiplayerManager.OnAlignmentMarkerChecked -= OnRelocalizationSucceeded;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log("[BallAndChainRealityManager] OnNetworkSpawn");

            if (IsServer)
            {
                _chain.SetActive(true);
                HoloKitHandTracker.Instance.IsActive = true;
            }
        }

        private void Update()
        {

        }

        private void OnRelocalizationSucceeded()
        {
            _chain.SetActive(true);
        }

        public void SpawnDynamicallySpawnedBall(Vector3 position, Quaternion rotation)
        {
            var ballInstance = Instantiate(_dynamicallySpawnedBallPrefab, position, rotation);
            ballInstance.GetComponent<NetworkObject>().Spawn();
        }

        [ClientRpc]
        public void SpawnFireworkClientRpc()
        {
            Debug.Log("[RealityManager] SpawnFireworkClientRpc");
        }

        [ServerRpc]
        private void SpawnFireworkServerRpc()
        {

        }
    }
}
