using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
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

        [SerializeField] private GameObject _portalPrefab;

        [SerializeField] private float _dragonToPortalOffset = 2f;

        [SerializeField] private GameObject _dragonPrefab;

        [SerializeField] private Transform _headTarget;

        public Transform HeadTarget => _headTarget;

        private void Awake()
        {
            MofaPlayer.OnMofaPlayerSpawned += OnMofaPlayerSpawned;
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

            if (HoloKitUtils.IsEditor && HoloKitApp.Instance.IsHost)
            {
                StartCoroutine(HoloKitAppUtils.WaitAndDo(1.2f, () =>
                {
                    SpawnDragonServerRpc(new Vector3(0f, 0f, 8f), Quaternion.Euler(0f, 180f, 0f));
                }));
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MofaPlayer.OnMofaPlayerSpawned -= OnMofaPlayerSpawned;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnDragonServerRpc(Vector3 position, Quaternion rotation)
        {
            SpawnPortalClientRpc(position, rotation);
            Vector3 dragonPosition = position - rotation * new Vector3(0f, 0f, _dragonToPortalOffset);
            var dragon = Instantiate(_dragonPrefab, dragonPosition, rotation);
            var dragonController = dragon.GetComponent<UnkaDragonController>();
            dragonController.ClipPlane = rotation * -Vector3.forward;
            dragonController.ClipPlaneHeihgt = position.magnitude;
            dragon.GetComponent<NetworkObject>().Spawn();
        }

        [ClientRpc]
        private void SpawnPortalClientRpc(Vector3 position, Quaternion rotation)
        {
            var portal = Instantiate(_portalPrefab, position, rotation);
            Destroy(portal, 10f);
        }

        private void OnMofaPlayerSpawned(MofaPlayer mofaPlayer)
        {
            if (mofaPlayer.OwnerClientId == 0)
            {
                _headTarget.GetComponent<FollowMovementManager>().FollowTarget = mofaPlayer.transform;
            }
        }

        //public void CreatePortalAndDragon(Vector3 targetPosOnFloor)
        //{
        //    var portalPos = targetPosOnFloor;
        //    var portalInstance = Instantiate(_portalPrefab);
        //    portalInstance.transform.position = portalPos;
        //    portalInstance.transform.LookAt(portalInstance.transform.position + DirectionHorizental(portalInstance.transform.position, HoloKit.HoloKitCamera.Instance.CenterEyePose.position));

        //    var dragonPos = portalPos - 2 * portalInstance.transform.forward;
        //    //var dragonTarget = portalPos + 2 * portalInstance.transform.forward;
        //    var dragonInstance = Instantiate(_dragonPrefab, _dragonContainer);
        //    dragonInstance.GetComponent<UnkaDragonController>().ClipPlane = -portalInstance.transform.forward;
        //    dragonInstance.GetComponent<UnkaDragonController>().ClipPlaneHeihgt = portalInstance.transform.position.magnitude;
        //    dragonInstance.transform.position = dragonPos;
        //}

        Vector3 DirectionHorizental(Vector3 pos, Vector3 target)
        {
            var horizentalTarget = new Vector3(target.x, pos.y ,target.z);
            return (horizentalTarget - pos).normalized;
        }
    }
}
