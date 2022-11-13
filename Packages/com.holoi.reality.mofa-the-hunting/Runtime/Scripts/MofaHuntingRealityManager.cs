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

        [SerializeField] private GameObject _portalPrefab;

        [SerializeField] private GameObject _dragonPrefab;

        [SerializeField] private Transform _dragonHeadTarget;

        [SerializeField] private Transform _dragonBodyTarget;

        public Transform DragonHeadTarget => _dragonHeadTarget;

        public Transform DragonBodyTarget => _dragonBodyTarget;

        private UnkaDragonController _dragonController;

        private void Awake()
        {
            MofaPlayer.OnMofaPlayerSpawned += OnMofaPlayerSpawned;
            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
        }

        protected override void Start()
        {
            base.Start();

            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                _arRaycastManager.enabled = true;
                _arPlacementIndicator.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementIndicator.gameObject);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MofaPlayer.OnMofaPlayerSpawned -= OnMofaPlayerSpawned;
            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnDragonServerRpc(Vector3 position, Quaternion rotation)
        {
            SpawnPortalClientRpc(position, rotation);
            Vector3 dragonPosition = position - rotation * new Vector3(0f, 0f, 2f);
            var dragon = Instantiate(_dragonPrefab, dragonPosition, rotation);
            dragon.GetComponent<NetworkObject>().SpawnWithOwnership(999);
        }

        [ClientRpc]
        private void SpawnPortalClientRpc(Vector3 position, Quaternion rotation)
        {
            var portal = Instantiate(_portalPrefab, position, rotation);
            Destroy(portal, 10f);
        }

        public void SetDragonController(UnkaDragonController dragonController)
        {
            _dragonController = dragonController;
        }

        private void OnMofaPlayerSpawned(MofaPlayer mofaPlayer)
        {
            if (mofaPlayer.OwnerClientId == 0)
            {
                _dragonHeadTarget.GetComponent<FollowMovementManager>().FollowTarget = mofaPlayer.transform;
            }
        }

        private void OnTriggered()
        {
            if (HoloKitApp.Instance.IsHost || HoloKitApp.Instance.IsPuppeteer)
            {
                if (_arPlacementIndicator != null)
                {
                    if (_arPlacementIndicator.IsActive && _arPlacementIndicator.IsValid)
                    {
                        SpawnDragonServerRpc(_arPlacementIndicator.HitPoint.position, _arPlacementIndicator.HitPoint.rotation);
                        _arPlacementIndicator.OnDeathFunc();
                        StartCoroutine(StartRoundFlow());
                    }
                    else
                    {
                        Debug.Log("[MOFATheHunting] Cannot spawn the dragon at the current target position");
                    }
                }
            }
        }
    }
}
