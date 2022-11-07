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
    public class BuddhasRealityManager : RealityManager
    {
        [Header("AR Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] ARRaycastManager _arRaycastManager;
        [SerializeField] ARPlaneManager _arPlaneManager;
        [SerializeField] AROcclusionManager _arOcclusionManager;

        [Header("Hand Objects")]
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _handHookHead;
        [SerializeField] Transform _handLoadedVFXParent;

        [Header("Reality Objects")]
        [SerializeField] GameObject _buddhasPrefab;
         GameObject _buddhasInstance;

        [Header("AR UI Components")]
        [SerializeField] LoadButtonController _placementLoadButton;
        [SerializeField] ARRayCastController _arRaycastController;
        [SerializeField] RaycastPlacmentVisualController _raycastVisualController;

        bool _isCastOnFloor = false;

        bool _isObjectInitialized = false;

        [HideInInspector] public Vector3 CastOnFloorPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // update hand hook's head position
                var dir = (_handJoint.position - _centerEye.position).normalized;

                _handHookHead.position = _handJoint.position + dir * 0.5f;
            }

            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono)
            {
                _arOcclusionManager.enabled = true;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            }
            else
            {
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
            _buddhasInstance = GameObject.Instantiate(_buddhasPrefab);

            _buddhasInstance.transform.position = _arRaycastController.transform.position;

            var target = new Vector3(_centerEye.position.x, _buddhasInstance.transform.position.y, _centerEye.position.z);

            var eyeHorizentalForward = GetHorizontalForward(_centerEye);

            target = _buddhasInstance.transform.position - eyeHorizentalForward;

            _buddhasInstance.transform.LookAt(target);

            //_buddhasInstance.GetComponent<NetworkObject>().Spawn();
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
    }
}