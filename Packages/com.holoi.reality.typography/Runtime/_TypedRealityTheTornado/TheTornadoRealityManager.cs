using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class TheTornadoRealityManager : RealityManager
    {
        [Header("The Tornado")]
        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private GameObject _tornado;

        [SerializeField] private float _raycastHorizontalOffset;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastManager.enabled = true;
            }
        }

        private void Update()
        {
            if (_arRaycastManager.enabled)
            {
                Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
                Vector3 horizontalForward = new Vector3(centerEyePose.position.x, 0f, centerEyePose.position.z).normalized;
                Vector3 rayOrigin = centerEyePose.position + _raycastHorizontalOffset * horizontalForward;
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            _tornado.transform.position = new Vector3(_tornado.transform.position.x,
                                                                      hit.pose.position.y,
                                                                      _tornado.transform.position.z);
                            return;
                        }
                    }
                }
            }
        }
    }
}