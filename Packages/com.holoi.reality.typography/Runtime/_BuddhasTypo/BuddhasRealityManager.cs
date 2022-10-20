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
        [SerializeField] PhaseManager _phaseManager;
        [SerializeField] ARRaycastManager _arRaycastManager;

        [Header("Reality Objects")]
        [SerializeField] GameObject _buddhasPrefab;
         GameObject _buddhasInstance;


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
            UpdateFloorHeight();
        }

        void InitializeRealityObject()
        {
            _buddhasInstance = GameObject.Instantiate(_buddhasPrefab);
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
                        CastOnFloorPosition = hitResult.pose.position;
                        transform.position = CastOnFloorPosition;
                        _isCastOnFloor = true;
                        return;
                    }
                }
                _isCastOnFloor = false;
            }
            else
            {
                _isCastOnFloor = false;
            }
        }
    }
}