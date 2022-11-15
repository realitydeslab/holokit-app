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

        [SerializeField] private GameObject _arPlacementIndicatorVisual;

        [SerializeField] private GameObject _invisibleFloorPrefab;

        [SerializeField] private GameObject _theDragonPrefab;

        [SerializeField] private float _dragonSpawnOffsetY;

        private GameObject _invisibleFloor;

        private TheDragonController _theDragonController;

        protected override void Start()
        {
            base.Start();
            HoloKitAppUIEventManager.OnTriggered += OnStarUITriggered;
            UI.MofaHuntingUIPanel.OnSpawnDragonButtonPressed += OnStarUITriggered;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                _arRaycastManager.enabled = true;
                _arPlacementIndicator.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_arPlacementIndicatorVisual);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            HoloKitAppUIEventManager.OnTriggered -= OnStarUITriggered;
            UI.MofaHuntingUIPanel.OnSpawnDragonButtonPressed -= OnStarUITriggered;
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
                Vector3 position = _arPlacementIndicator.HitPoint.position;
                Quaternion rotation = _arPlacementIndicator.HitPoint.rotation;
                _arRaycastManager.enabled = false;
                _arPlacementIndicator.OnPlacedFunc();
                StartRoundServerRpc(position, rotation);
            }
            else
            {
                Debug.Log("[MofaHuntingRealityManager] Failed to start round");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartRoundServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
        {
            SpawnInvisibleFloorClientRpc(position.y);
            SpawnTheDragon(position, rotation, serverRpcParams.Receive.SenderClientId);
            if (_arRaycastManager.enabled)
            {
                _arRaycastManager.enabled = false;
                _arPlacementIndicator.OnDisabledFunc();
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

        private void SpawnTheDragon(Vector3 position, Quaternion rotation, ulong ownerClientId)
        {
            var theDragon = Instantiate(_theDragonPrefab, position + new Vector3(0f, _dragonSpawnOffsetY, 0f), rotation);
            theDragon.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        public void SetTheDragonController(TheDragonController theDragonController)
        {
            _theDragonController = theDragonController;
        }
    }
}
