using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class ARPlacementIndicator : MonoBehaviour
    {
        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private Transform _hitPoint;

        [Header("Parameters")]
        [SerializeField] private bool _isActive;

        [SerializeField] private float _horizontalRaycastOffset;

        [SerializeField] private float _horizontalPlacementOffset;

        /// <summary>
        /// From the center eye to the point.
        /// </summary>
        [SerializeField] private Vector3 _hitPositionDefaultOffset;

        /// <summary>
        /// We make the y position of hit point slightly higher than the ground plane,
        /// so the indicator is not occluded by the ground plane.
        /// </summary>
        [SerializeField] private float _hitPointGroundOffset;

        [SerializeField] private float _destroyDelay = 0.3f;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
            }
        }

        public bool IsValid => _hitPoint.gameObject.activeSelf;

        public Transform HitPoint => _hitPoint;

        public UnityEvent OnFoundPlane;

        public UnityEvent OnLostPlane;

        public UnityEvent OnDeath;

        private void Start()
        {
            _hitPoint.position = _hitPositionDefaultOffset;
            _hitPoint.rotation = Quaternion.Euler(0f, 180f, 0f);
            _hitPoint.gameObject.SetActive(false);

            if (HoloKitUtils.IsEditor)
            {
                StartCoroutine(HoloKitAppUtils.WaitAndDo(1.2f, () =>
                {
                    _hitPoint.gameObject.SetActive(true);
                    OnFoundPlane?.Invoke();
                }));
            }
        }

        private void Update()
        {
            if (HoloKitUtils.IsEditor) { return; }

            if (_isActive && _arRaycastManager != null && _arRaycastManager.enabled)
            {
                Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
                Vector3 horizontalForward = new Vector3(centerEyePose.forward.x, 0f, centerEyePose.forward.z).normalized;
                Vector3 rayOrigin = centerEyePose.position + _horizontalPlacementOffset * horizontalForward;
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            Vector3 position = centerEyePose.position + _horizontalRaycastOffset * horizontalForward;
                            Vector3 hitPosition = new(position.x, hit.pose.position.y, position.z);
                            _hitPoint.SetPositionAndRotation(hitPosition + _hitPointGroundOffset * Vector3.up,
                                                             Quaternion.Euler(0f, centerEyePose.rotation.eulerAngles.y, 0f) * Quaternion.Euler(180f * Vector3.up));
                            if (!_hitPoint.gameObject.activeSelf)
                            {
                                _hitPoint.gameObject.SetActive(true);
                                OnFoundPlane?.Invoke();
                                Debug.Log("[ARPlacementIndicator] OnFoundPlane");
                            }
                            return;
                        }
                    }
                }
                // Set ARRaycastPoint to its default pose relative to the center eye
                _hitPoint.SetPositionAndRotation(centerEyePose.position + centerEyePose.TransformVector(_hitPositionDefaultOffset),
                                                 Quaternion.Euler(0f, centerEyePose.rotation.eulerAngles.y, 0f) * Quaternion.Euler(180f * Vector3.up));
                if (_hitPoint.gameObject.activeSelf)
                {
                    _hitPoint.gameObject.SetActive(false);
                    OnLostPlane?.Invoke();
                    Debug.Log("[ARPlacementIndicator] OnLostPlane");
                }
            }
        }

        /// <summary>
        /// Call this function instead of destroy the gameObject.
        /// </summary>
        public void OnDeathFunc()
        {
            _isActive = false;
            OnDeath?.Invoke();
            Destroy(gameObject, _destroyDelay);
        }
    }
}
