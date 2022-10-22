using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppARSessionManager : MonoBehaviour
    {
        [Header("Plane Detection")]
        [SerializeField] private ARPlane _arPlanePrefab;

        [Header("Image Tracking")]
        [SerializeField] private XRReferenceImageLibrary _holokitAppReferenceImageLibrary;

        [SerializeField] private GameObject _trackedImagePrefab;

        private ARTrackedImageManager _arTrackedImageManager;

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
                // Add ARTrackedImageManager if client does not have ont
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
    }
}
