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
    public class BodyTrackingRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] PhaseManager _phaseManager;

        ARRaycastManager _arRaycastManager;
        
        bool _isRaycastHitFloor = false;

        BoneController _bone;
        bool _isBodyValid = false;
        bool _isTrigger = false;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if(_centerEye==null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
            if (FindObjectOfType<BoneController>() != null && !_isTrigger)
            {
                _isBodyValid = true;
                _bone = FindObjectOfType<BoneController>();
                _phaseManager.PlayPhaseSource();
                _isTrigger = true;
            }

            UpdateFloorHeight();

        }

        void UpdateFloorHeight()
        {

            Vector3 rayOrigin = _centerEye.position + _centerEye.forward * 1f;

            Ray ray = new(rayOrigin, Vector3.down);

            List<ARRaycastHit> hitResults = new();

            if (_arRaycastManager.Raycast(ray, hitResults, TrackableType.Planes))
            {
                foreach (var hitResult in hitResults)
                {
                    var arPlane = hitResult.trackable.GetComponent<ARPlane>();

                    if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                    {
                        HitPosition = hitResult.pose.position;
                        transform.position = HitPosition;
                        _isRaycastHitFloor = true;
                        return;
                    }
                }
                _isRaycastHitFloor = false;
                //transform.position = _centerEye.position + horizontalForward.normalized * 1.5f + (transform.up * -1f);

            }
            else
            {
                _isRaycastHitFloor = false;
                //transform.position = _centerEye.position + horizontalForward.normalized * 1.5f + (transform.up * -1f);
            }
        }
    }
}