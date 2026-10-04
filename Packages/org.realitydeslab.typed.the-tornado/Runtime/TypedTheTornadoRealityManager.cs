// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;
using HoloKit;

namespace RealityDesignLab.Typed.TheTornado
{
    public class TypedTheTornadoRealityManager : RealityManager
    {
        [Header("The Tornado")]
        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private GameObject _tornado;

        [SerializeField] private float _raycastHorizontalOffset;

        public NetworkVariable<float> GroundPosY = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastManager.enabled = true;
            }
        }

        /// <summary>The downward ray that finds the floor under the tornado, a little ahead of the player.</summary>
        public static Ray FloorRay(Transform centerEyePose, float horizontalOffset)
        {
            Vector3 horizontalForward = Vector3.ProjectOnPlane(centerEyePose.forward, Vector3.up).normalized;
            return new Ray(centerEyePose.position + horizontalOffset * horizontalForward, Vector3.down);
        }

        private void Update()
        {
            if (_arRaycastManager.enabled)
            {
                Ray ray = FloorRay(HoloKitCameraManager.Instance.CenterEyePose, _raycastHorizontalOffset);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            GroundPosY.Value = hit.pose.position.y;
                            return;
                        }
                    }
                }
            }
        }
    }
}