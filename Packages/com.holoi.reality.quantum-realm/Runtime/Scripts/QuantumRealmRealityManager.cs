using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.QuantumRealm
{
    public class QuantumRealmRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private GameObject _arRaycastPoint;

        [SerializeField] private ARRaycastIndicatorController _arRaycastIndicatorController;

        [SerializeField] private GameObject _startButton;

        [Header("Hand")]
        public Transform HandTransform; // This is networked

        [Header("Apple")]
        public CoreHapticsManager CoreHapticsManager;

        public PhaseManager PhaseManager;

        [Header("Quantum Realm")]
        [SerializeField] private NetworkObject _buddhaGroupPrefab;

        private BuddhaGroup _buddhaGroup;

        private const float HorizontalRaycastOffset = 2f;

        private readonly Vector3 ARRaycastPointDefaultOffset = new(0f, -0.6f, 2f);

        // We make the y position of ARRaycastPoint slightly higher than the ground plane,
        // so the indicator is not occluded by the ground plane
        private const float ARRaycastPointGroundOffset = 0.05f;

        [Header("Events")]
        public UnityEvent OnARRaycastManagerFoundPlane;

        public UnityEvent OnARRaycastManagerLostPlane;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.enabled = true;
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                HoloKitHandTracker.Instance.Active = true;

                _arRaycastPoint.transform.position = ARRaycastPointDefaultOffset;
                _arRaycastPoint.SetActive(false);
                // For debug
                if (HoloKitUtils.IsEditor)
                {
                    StartCoroutine(HoloKitAppUtils.WaitAndDo(2.7f, () =>
                    {
                        _arRaycastPoint.SetActive(true);
                        OnARRaycastManagerFoundPlane?.Invoke();
                    }));
                }
            }
            else
            {
                // Delete unnecessary objects on client at the very beginning
                Destroy(_arRaycastPoint);
                Destroy(_arRaycastIndicatorController.gameObject);
                Destroy(_startButton);
                Destroy(CoreHapticsManager.gameObject);
            }
        }

        public void OnSessionStarted()
        {
            // Spawn the buddha group
            SpawnBuddhaGroup();

            // Turn off indicators and buttons
            _startButton.GetComponent<LoadButtonController>().OnDeath();
            _arRaycastIndicatorController.OnDeath();
            Destroy(_arRaycastPoint);

            // Turn off plane detection and raycast
            HoloKitApp.Instance.ARSessionManager.SetARPlaneManagerActive(false);
            HoloKitApp.Instance.ARSessionManager.SetARRaycastManagerActive(false);

            Debug.Log("[QuantumRealmRealityManager] Buddha group spawned");
        }

        private void SpawnBuddhaGroup()
        {
            Vector3 position = new(HoloKitCamera.Instance.CenterEyePose.transform.position.x,
                                   _arRaycastPoint.transform.position.y,
                                   HoloKitCamera.Instance.CenterEyePose.transform.position.z);
            var buddhaGroup = Instantiate(_buddhaGroupPrefab, position, _arRaycastPoint.transform.rotation);
            buddhaGroup.Spawn();
        }

        public void SetBuddhaGroup(BuddhaGroup buddhaGroup)
        {
            _buddhaGroup = buddhaGroup;
        }

        private void Update()
        {
            if (HoloKitUtils.IsEditor) { return; }

            if (!HoloKitApp.Instance.IsHost) { return; }

            // Wo do raycast when buddha group has not been spawned
            if (_buddhaGroup == null && _arRaycastPoint != null && _arRaycastManager.enabled)
            {
                Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
                Vector3 hozizontalForward = new Vector3(centerEyePose.forward.x, 0f, centerEyePose.forward.z).normalized;
                Vector3 rayOrigin = centerEyePose.position + HorizontalRaycastOffset * hozizontalForward;
                Ray ray = new(rayOrigin, Vector3.down);
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    foreach (var hit in hits)
                    {
                        var arPlane = hit.trackable.GetComponent<ARPlane>();
                        if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                        {
                            _arRaycastPoint.transform.SetPositionAndRotation(hit.pose.position + new Vector3(0f, ARRaycastPointGroundOffset, 0f),
                                Quaternion.Euler(0f, centerEyePose.rotation.eulerAngles.y, 0f));
                            if (!_arRaycastPoint.activeSelf)
                            {
                                _arRaycastPoint.SetActive(true);
                                OnARRaycastManagerFoundPlane?.Invoke();
                            }
                            return;
                        }
                    }
                }

                // Set ARRaycastPoint to its default pose relative to the center eye
                _arRaycastPoint.transform.SetPositionAndRotation(centerEyePose.position + centerEyePose.TransformVector(ARRaycastPointDefaultOffset),
                                                                 centerEyePose.rotation);
                if (_arRaycastPoint.activeSelf)
                {
                    _arRaycastPoint.SetActive(false);
                    OnARRaycastManagerLostPlane?.Invoke();
                }
            }
        }
    }
}