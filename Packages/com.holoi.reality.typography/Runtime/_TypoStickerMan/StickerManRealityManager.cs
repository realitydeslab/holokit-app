using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using Unity.Netcode;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Holoi.Reality.Typography
{
    public class StickerManRealityManager : RealityManager
    {
        [Header("AR Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] ARRaycastManager _arRaycastManager;
        [SerializeField] AROcclusionManager _arOcclusionManager;
        [SerializeField] ARPlaneManager _arPlaneManager;
        [SerializeField] Material _arShadowedPlaneMat;
        [SerializeField] GameObject _arShadowedPlane;


        [Header("Hand Objects")]
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _handHookHead;
        [SerializeField] Transform _handLoadedVFXParent;
        [SerializeField] HandObject _ho;

        [Header("AR NetWork Base Objects")]
        [SerializeField] Transform _serverCenterEye;

        [Header("Reality Objects")]
        [SerializeField] GameObject _prefab;
        GameObject _prefabInstance;

        [Header("AR UI Components")]
        [SerializeField] LoadButtonController _placementLoadButton;
        [SerializeField] ARRayCastController _arRaycastController;
        [SerializeField] RaycastPlacmentVisualController _raycastVisualController;

        [Header("PHASE")]
        [SerializeField] PhaseManager _phaseManager;

        bool _isCastOnFloor = false;

        bool _isObjectInitialized = false;

        [HideInInspector] public Vector3 CastOnFloorPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void OnEnable()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged += ToMono;
        }

        private void OnDisable()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged -= ToMono;
        }

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastManager.enabled = true;
                _arPlaneManager.enabled = true;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = true;

                HoloKitHandTracker.Instance.Active = true;
                HandObject.Instance.IsSyncedHand = false;
                ARRayCastController.Instance.enabled = true;
            }
            else
            {
                _arRaycastManager.enabled = false;
                _arPlaneManager.enabled = false;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = false;


                HoloKitHandTracker.Instance.Active = false;
                HandObject.Instance.IsSyncedHand = true;
                ARRayCastController.Instance.enabled = false;
            }

            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // update hand hook's head position
                var dir = (_handJoint.position - _centerEye.position).normalized;

                _handHookHead.position = _handJoint.position + dir * 0.025f;

                SyncHandValidStateCLientRpc(_ho.IsValid);
            }
        }

        void ToMono(HoloKitRenderMode mode)
        {
            if (mode == HoloKitRenderMode.Mono)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            }
            else
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
            }
        }

        public static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }

        public void InitializeRealityObject()
        {
            Debug.Log("InitializeRealityObject");
            _prefabInstance = Instantiate(_prefab);

            _prefabInstance.transform.position = _arRaycastController.transform.position;

            //var eyeHorizentalForward = GetHorizontalForward(_centerEye);

            var lookAtPoint = _prefabInstance.transform.position;

            _prefabInstance.transform.LookAt(lookAtPoint);

            _prefabInstance.GetComponent<NetworkObject>().Spawn();

            SwitchARPlaneToShadowed();
        }

        IEnumerator DisableGameObjectAfterTimes(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }

        public void DisableARPlaneManager()
        {
            _arPlaneManager.enabled = false;
            _arPlaneManager.enabled = false;
            var planeList = FindObjectsOfType<ARPlane>();
            foreach (var plane in planeList)
            {
                Destroy(plane.gameObject);
            }
        }

        public void DisableARRaycastManager()
        {
            // disble ar ui script:
            _arRaycastController.enabled = false;
            // play die animation
            _raycastVisualController.PlayDie();
            // disable go
            StartCoroutine(DisableGameObjectAfterTimes(_arRaycastController.gameObject, 2f));
        }

        public void OnInteractionTriggered()
        {
            TriggerHandVFX();
        }

        void TriggerHandVFX()
        {
            _handLoadedVFXParent.gameObject.SetActive(true);
            StartCoroutine(DisableGameObjectAfterTimes(_handLoadedVFXParent.gameObject, 2.5f));
        }

        public void SetPlacementLoadButton(bool state)
        {
            //Debug.Log("SetPlacementLoadButton: " + state);
            if (HoloKitApp.Instance.IsHost)
            {
                _placementLoadButton.gameObject.SetActive(state);
            }
        }


        [ClientRpc]
        public void SyncHandValidStateCLientRpc(bool valid)
        {
            if (HoloKitApp.Instance.IsHost)
            {
                return;
            }
            else
            {
                if (_ho.IsSyncedHand)
                {
                    _ho.IsValid = valid;
                }
            }
        }

        public void SwitchARPlaneToShadowed()
        {
            // on host:

            //_arPlaneManager.enabled = false;

            _arPlaneManager.planePrefab = _arShadowedPlane;

            //var planeList = FindObjectsOfType<ARPlane>();
            //foreach (var plane in planeList)
            //{
            //    plane.GetComponent<MeshRenderer>().material = _arShadowedPlaneMat;
            //}

            // on client: 
            SwitchARPlaneToShadowedCLientRpc();
        }

        [ClientRpc]
        public void SwitchARPlaneToShadowedCLientRpc()
        {
            _arPlaneManager.planePrefab = _arShadowedPlane;
        }
    }
}