using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppARSessionManager : MonoBehaviour
    {
        [Header("Image Tracking")]
        [SerializeField] private XRReferenceImageLibrary _holokitAppReferenceImageLibrary;

        [SerializeField] private GameObject _trackedImagePrefab;

        [Header("Plane Detection")]
        [SerializeField] private ARPlane _arPlanePrefab;

        public ARTrackedImageManager ARTrackedImageManager => _arTrackedImageManager;

        public ARPlaneManager ARPlaneManager => _arPlaneManager;

        public ARRaycastManager ARRaycastManager => _arRaycastManager;

        private ARTrackedImageManager _arTrackedImageManager;

        private ARPlaneManager _arPlaneManager;

        private ARRaycastManager _arRaycastManager;

        private void Start()
        {
            var xrOrigin = HoloKitCamera.Instance.GetComponentInParent<XROrigin>();

            // Image Tracking
            if (xrOrigin.TryGetComponent(out _arTrackedImageManager))
            {
                // Host does not need image tracker
                if (HoloKitApp.Instance.IsHost)
                {
                    Destroy(_arTrackedImageManager);
                }
                else
                {
                    SetupARTrackedImageManager();
                }
            }
            else
            {
                // Add ARTrackedImageManager if client does not have one
                if (!HoloKitApp.Instance.IsHost)
                {
                    _arTrackedImageManager = xrOrigin.gameObject.AddComponent<ARTrackedImageManager>();
                    SetupARTrackedImageManager();
                }
            }
        }

        private void SetupARTrackedImageManager()
        {
            _arTrackedImageManager.enabled = false;
            _arTrackedImageManager.referenceLibrary = _holokitAppReferenceImageLibrary;
            _arTrackedImageManager.requestedMaxNumberOfMovingImages = 1;
            _arTrackedImageManager.trackedImagePrefab = _trackedImagePrefab;
        }

        public void SetARTrackedImageManagerActive(bool active)
        {
            if (_arTrackedImageManager == null) { return; }

            foreach (var trackable in _arTrackedImageManager.trackables)
            {
                trackable.gameObject.SetActive(active);
            }
            _arTrackedImageManager.enabled = active;
        }

        public void AddARPlaneManager()
        {
            var xrOrigin = HoloKitCamera.Instance.GetComponentInParent<XROrigin>();

            if (xrOrigin.TryGetComponent<ARPlaneManager>(out var _))
            {
                Debug.Log("[HoloKitAppARSessionManager] ARPlaneManager already added");
                return;
            }

            _arPlaneManager = xrOrigin.gameObject.AddComponent<ARPlaneManager>();
            _arPlaneManager.planePrefab = _arPlanePrefab.gameObject;
            _arPlaneManager.enabled = true;
        }

        public void SetARPlaneManagerActive(bool active)
        {
            if (_arPlaneManager == null) { return; }

            _arPlaneManager.enabled = active;
            if (!active)
            {
                // Destory all detected ARPlane objects
                DestroyExistingARPlanes();
            }
        }

        private void DestroyExistingARPlanes()
        {
            var planes = FindObjectsOfType<ARPlane>();
            foreach (var plane in planes)
            {
                Destroy(plane.gameObject);
            }
        }

        public void AddARRaycastManager()
        {
            var xrOrigin = HoloKitCamera.Instance.GetComponentInParent<XROrigin>();

            if (xrOrigin.TryGetComponent<ARRaycastManager>(out var _))
            {
                Debug.Log("[HoloKitAppARSessionManager] ARRaycastManager already added");
                return;
            }

            _arRaycastManager = xrOrigin.gameObject.AddComponent<ARRaycastManager>();
            _arRaycastManager.enabled = true;
        }

        public void SetARRaycastManagerActive(bool active)
        {
            if (_arRaycastManager == null) { return; }

            _arRaycastManager.enabled = active;
        }
    }
}
