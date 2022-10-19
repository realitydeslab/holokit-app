using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Holoi.Reality.Typography
{
    public class PuppetryRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] GameObject _arSoftShadowPlane;

        ARRaycastManager _arRaycastManager;
        ARPlaneManager _arPlaneManager;

        bool _isRaycastHitFloor = false;
        bool _isFloorHeightValid = false;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
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
                        _isFloorHeightValid = true;
                        return;
                    }
                }
                _isRaycastHitFloor = false;

            }
            else
            {
                _isRaycastHitFloor = false;
            }
        }
    }
}