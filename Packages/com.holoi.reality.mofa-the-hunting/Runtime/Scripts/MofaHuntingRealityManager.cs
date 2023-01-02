using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;

namespace Holoi.Reality.MOFATheHunting
{
    public partial class MofaHuntingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Hunting")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementManager;

        [SerializeField] private GameObject _arPlacementIndicator;

        [SerializeField] private GameObject _invisibleFloorPrefab;

        [SerializeField] private GameObject _portalPrefab;

        [SerializeField] private GameObject _dragonPrefab;

        [SerializeField] private float _portalSpawnOffsetY;

        [SerializeField] private float _dragonSpawnOffsetY;

        public PortalController PortalController => _portalController;

        public DragonController DragonController => _dragonController;

        private GameObject _invisibleFloor;

        private PortalController _portalController;

        private DragonController _dragonController;

        public static event Action OnFailedToSpawnDragonAtCurrentPosition;

        /// <summary>
        /// Sent when the dragon is spawned.
        /// </summary>
        public static event Action OnDragonSpawned;

        /// <summary>
        /// Sent when the dragon is slayed.
        /// </summary>
        public static event Action OnDragonDied;

        private void Start()
        {
            UI.MofaHuntingDragonControllerUIPanel.OnSpawnDragonButtonPressed += OnSpawnDragonButtonPressed;
            LockTargetSystem_Init();

            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                _arPlacementManager.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementManager.gameObject);
                Destroy(_arPlacementIndicator);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UI.MofaHuntingDragonControllerUIPanel.OnSpawnDragonButtonPressed -= OnSpawnDragonButtonPressed;
            LockTargetSystem_Deinit();
        }

        private void Update()
        {
            LockTargetSystem_Update();
        }

        private void OnSpawnDragonButtonPressed()
        {
            TryGetReady();
        }

        public override void TryGetReady()
        {
            // Only the host can get ready in Hunting
            if (!IsServer)
                return;

            if (_arPlacementManager.IsValid)
            {
                foreach (var mofaPlayer in MofaPlayerList)
                {
                    mofaPlayer.Ready.Value = true;
                }
            }
            else
            {
                OnFailedToSpawnDragonAtCurrentPosition?.Invoke();
            }
        }

        protected override void OnMofaPlayerReadyChanged(MofaPlayer mofaPlayer)
        {
            if (!IsServer) return;

            if (mofaPlayer.Ready.Value)
            {
                var mofaPlayerList = MofaPlayerList;
                // We do not need at least one player in each team to start
                // If all players are ready
                int readyPlayerCount = mofaPlayerList.Count(t => t.Ready.Value);
                if (readyPlayerCount == mofaPlayerList.Count)
                {
                    SetupRound();
                    StartRound();
                }
            }
        }

        protected override void SetupRound()
        {
            SetupLifeShieldsForDragonSlayers();
            ResetPlayerStats();

            // Spawn the dragon
            Vector3 position = _arPlacementManager.HitPoint.position;
            Quaternion rotation = _arPlacementManager.HitPoint.rotation;
            _arPlacementManager.OnDisabledFunc();
            _arPlaneManager.enabled = false;
            _arRaycastManager.enabled = false;

            SpawnInvisibleFloor(position.y);
            SpawnDragon(position, rotation);
        }

        // Host only
        protected override void StartRound()
        {
            StartCoroutine(StartHuntingFlow());
        }

        private void SpawnInvisibleFloor(float posY)
        {
            if (_invisibleFloor != null) { return; }
            _invisibleFloor = Instantiate(_invisibleFloorPrefab, new Vector3(0f, posY, 0f), Quaternion.identity);
            _invisibleFloor.GetComponent<NetworkObject>().Spawn();
        }

        private void SpawnDragon(Vector3 position, Quaternion rotation)
        {
            if (_dragonController != null) { return; }
            // Spawn portal
            var portal = Instantiate(_portalPrefab, position + new Vector3(0f, _portalSpawnOffsetY, 0f), rotation);
            portal.GetComponent<NetworkObject>().Spawn();
            // Spawn dragon
            var theDragon = Instantiate(_dragonPrefab, position + new Vector3(0f, _dragonSpawnOffsetY, 0f) - 2f * (rotation * Vector3.forward), rotation);
            theDragon.GetComponent<NetworkObject>().Spawn();

            OnDragonSpawned?.Invoke();
        }

        public void SetInvisibleFloor(GameObject floor)
        {
            _invisibleFloor = floor;
        }

        public void SetPortalController(PortalController portalController)
        {
            _portalController = portalController;
        }

        public void SetTheDragonController(DragonController theDragonController)
        {
            _dragonController = theDragonController;
        }

        /// <summary>
        /// We do not spawn life shield for the host, who is the dragon controller.
        /// </summary>
        private void SetupLifeShieldsForDragonSlayers()
        {
            var mofaPlayerList = MofaPlayerList;
            foreach (var mofaPlayer in mofaPlayerList)
            {
                if (mofaPlayer.OwnerClientId == 0)
                    continue;

                if (mofaPlayer.LifeShield == null)
                    SpawnLifeShield(mofaPlayer.OwnerClientId);
                else
                    mofaPlayer.LifeShield.Renovate();
            }
        }

        private IEnumerator StartHuntingFlow()
        {
            yield return null;
            CurrentPhase.Value = MofaPhase.Countdown;
            yield return new WaitForSeconds(CountdownDuration);
            CurrentPhase.Value = MofaPhase.Fighting;
        }

        public void OnDragonDead()
        {
            StartCoroutine(OnDragonDeadCoroutine());
        }

        private IEnumerator OnDragonDeadCoroutine()
        {
            yield return null;
            CurrentPhase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            CurrentPhase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            CurrentPhase.Value = MofaPhase.Waiting;
            // Let the host to spawn a new dragon
            _arPlaneManager.enabled = true;
            _arRaycastManager.enabled = true;
            _arPlacementManager.OnRestartFunc();
        }
    }
}
