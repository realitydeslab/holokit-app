// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.Typed.TheRain
{
    public class TypedTheRainRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [Header("Rain")]
        [SerializeField] private Transform _hostCenterEyeTransform;

        [SerializeField] private VisualEffect _rainVfx;

        private readonly NetworkVariable<Vector3> _hostPlaneHeight = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private float _lastRaycastTime;

        private const float RaycastInterval = 5f;

        private const float RaycastHorizontalOffset = 1.2f;

        private const float CenterEyeToChestOffsetY = 0.5f;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _hostPlaneHeight.OnValueChanged += OnHostPlaneHeightValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _hostPlaneHeight.OnValueChanged -= OnHostPlaneHeightValueChanged;
        }

        private void Update()
        {
            if (_arRaycastManager.enabled && Time.time - _lastRaycastTime > RaycastInterval)
            {
                Vector3 horizontalForward = GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);
                Vector3 rayOrigin = HoloKitCamera.Instance.CenterEyePose.position + horizontalForward * RaycastHorizontalOffset;
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            _hostPlaneHeight.Value = hit.pose.position;
                            _lastRaycastTime = Time.time;
                            return;
                        }
                    }
                }
            }
            UpdateRainVfx();
        }

        private void UpdateRainVfx()
        {
            _rainVfx.SetVector3("Head Position_position", _hostCenterEyeTransform.position);
            _rainVfx.SetVector3("Chest Position_position", _hostCenterEyeTransform.position + new Vector3(0f, -CenterEyeToChestOffsetY, 0f));
        }

        private void OnHostPlaneHeightValueChanged(Vector3 oldPosition, Vector3 newPosition)
        {
            _rainVfx.SetVector3("Plane Position_position", newPosition);
        }

        private static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }
    }
}