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
    public class ImitateTypoRealityManager : RealityManager
    {
        ARRaycastManager _arRaycastManager;
        Transform _centerEye;
        bool isHit = false;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

        }

        private void Start()
        {
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            _centerEye = HoloKitCamera.Instance.CenterEyePose;
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
                        isHit = true;
                        return;
                    }
                }
                isHit = false;
                //transform.position = _centerEye.position + horizontalForward.normalized * 1.5f + (transform.up * -1f);

            }
            else
            {
                isHit = false;
                //transform.position = _centerEye.position + horizontalForward.normalized * 1.5f + (transform.up * -1f);
            }
        }
    }
}