using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting
{
    public class MofaHuntingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Hunting")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementIndicator _arPlacementIndicator;

        [SerializeField] private GameObject _invisibleFloorPrefab;

        [SerializeField] private GameObject _theDragonPrefab;

        [SerializeField] private float _dragonSpawnOffsetY;

        private GameObject _invisibleFloor;

        protected override void Start()
        {
            base.Start();
            HoloKitAppUIEventManager.OnTriggered += OnStarUITriggered;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                _arRaycastManager.enabled = true;
                _arPlacementIndicator.IsActive = true;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            HoloKitAppUIEventManager.OnTriggered -= OnStarUITriggered;
        }

        private void OnStarUITriggered()
        {
            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
                {
                    TryStartRound();
                    return;
                }
            }
        }

        private void TryStartRound()
        {
            if (_arPlacementIndicator != null && _arPlacementIndicator.IsActive && _arPlacementIndicator.IsValid)
            {
                // Spawn the invisible ground plane
                SpawnInvisibleFloorClientRpc(_arPlacementIndicator.HitPoint.position.y);
                // Spawn the dragon
                SpawnTheDragon(_arPlacementIndicator.HitPoint.position, _arPlacementIndicator.HitPoint.rotation);
                _arRaycastManager.enabled = false;
                _arPlacementIndicator.OnPlacedFunc();
            }
            else
            {
                Debug.Log("[MofaHuntingRealityManager] Failed to start round");
            }
        }

        [ClientRpc]
        private void SpawnInvisibleFloorClientRpc(float posY)
        {
            if (_invisibleFloor != null) { return; }
            _invisibleFloor = Instantiate(_invisibleFloorPrefab, new Vector3(0f, posY, 0f), Quaternion.identity);
            if (HoloKitUtils.IsRuntime)
            {
                _invisibleFloor.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }

        private void SpawnTheDragon(Vector3 position, Quaternion rotation)
        {
            Instantiate(_theDragonPrefab, position + new Vector3(0f, _dragonSpawnOffsetY, 0f), rotation);
        }
    }
}
