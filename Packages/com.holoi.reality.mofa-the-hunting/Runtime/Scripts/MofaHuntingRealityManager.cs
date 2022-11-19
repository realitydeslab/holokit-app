using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;

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

        [SerializeField] private GameObject _portalPrefab;

        [SerializeField] private GameObject _theDragonPrefab;

        [SerializeField] private float _portalSpawnOffsetY;

        [SerializeField] private float _dragonSpawnOffsetY;

        public PortalController PortalController => _portalController;

        public TheDragonController TheDragonController => _theDragonController;

        private GameObject _invisibleFloor;

        private PortalController _portalController;

        private TheDragonController _theDragonController;

        protected override void Start()
        {
            base.Start();
            HoloKitAppUIEventManager.OnTriggered += OnStarUITriggered;
            UI.MofaHuntingUIPanel.OnSpawnDragonButtonPressed += OnStarUITriggered;

            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
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
            if (HoloKitApp.Instance.IsHost)
            {
                if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
                {
                    TryStartRound();
                    return;
                }
            }
        }

        // Host only
        private void TryStartRound()
        {
            if (_arPlacementIndicator != null && _arPlacementIndicator.IsActive && _arPlacementIndicator.IsValid)
            {
                Vector3 position = _arPlacementIndicator.HitPoint.position;
                Quaternion rotation = _arPlacementIndicator.HitPoint.rotation;
                _arPlaneManager.enabled = false;
                _arRaycastManager.enabled = false;
                _arPlacementIndicator.OnDisabledFunc();
                StartRound(position, rotation);
            }
            else
            {
                Debug.Log("[MofaHuntingRealityManager] Failed to start round");
            }
        }

        // Host only
        private void StartRound(Vector3 position, Quaternion rotation)
        {
            SpawnInvisibleFloor(position.y);
            SpawnTheDragon(position, rotation);
            SpawnLifeShieldsForNonHostPlayers();
            StartCoroutine(StartHuntingFlow());
        }

        private void SpawnInvisibleFloor(float posY)
        {
            if (_invisibleFloor != null) { return; }
            _invisibleFloor = Instantiate(_invisibleFloorPrefab, new Vector3(0f, posY, 0f), Quaternion.identity);
            _invisibleFloor.GetComponent<NetworkObject>().Spawn();
        }

        private void SpawnTheDragon(Vector3 position, Quaternion rotation)
        {
            if (_theDragonController != null) { return; }
            // Spawn portal
            var portal = Instantiate(_portalPrefab, position + new Vector3(0f, _portalSpawnOffsetY, 0f), rotation);
            portal.GetComponent<NetworkObject>().Spawn();
            // Spawn dragon
            var theDragon = Instantiate(_theDragonPrefab, position + new Vector3(0f, _dragonSpawnOffsetY, 0f) - 2f * (rotation * Vector3.forward), rotation);
            theDragon.GetComponent<NetworkObject>().Spawn();
        }

        public void SetInvisibleFloor(GameObject floor)
        {
            _invisibleFloor = floor;
        }

        public void SetPortalController(PortalController portalController)
        {
            _portalController = portalController;
        }

        public void SetTheDragonController(TheDragonController theDragonController)
        {
            _theDragonController = theDragonController;
        }

        private void SpawnLifeShieldsForNonHostPlayers()
        {
            foreach (ulong playerClientId in Players.Keys)
            {
                if (playerClientId == 0) { continue; }
                
                var lifeShield = Players[playerClientId].LifeShield;
                if (lifeShield != null)
                {
                    Destroy(lifeShield.gameObject);
                }
                SpawnLifeShield(playerClientId);
            }
        }

        private IEnumerator StartHuntingFlow()
        {
            CurrentPhase = MofaPhase.Countdown;
            yield return new WaitForSeconds(CountdownTime);
            CurrentPhase = MofaPhase.Fighting;
        }

        public void OnDragonDead()
        {
            StartCoroutine(OnDragonDeadCoroutine());
        }

        private IEnumerator OnDragonDeadCoroutine()
        {
            CurrentPhase = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            RoundResult = MofaRoundResult.RedTeamWins;
            CurrentPhase = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            CurrentPhase = MofaPhase.RoundData;
            // Let the host to spawn a new dragon
            _arPlaneManager.enabled = true;
            _arRaycastManager.enabled = true;
            _arPlacementIndicator.OnRestartFunc();
        }
    }
}
