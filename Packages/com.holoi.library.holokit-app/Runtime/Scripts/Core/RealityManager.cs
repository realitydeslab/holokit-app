using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;
using UnityEngine.XR.ARFoundation;

namespace Holoi.Library.HoloKitApp
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public string SceneName;

        public List<GameObject> NetworkPrefabs;

        private Vector3 _lastImagePosition;

        private Quaternion _lastImageRotation;

        private int _imageStablizationFrameCount = 0;

        private const float ImageStablizationPositionThreshold = 0.03f;

        private const float ImageStablizationRotationThreshold = 7;

        private const int ImageStabilizationFrameNum = 80;

        public static event Action<RealityManager> OnRealityManagerSpawned;

        public static event Action OnFinishedScanningQRCode;

        protected virtual void Awake()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator += OnConnectedAsSpectator;
        }

        public override void OnDestroy()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator -= OnConnectedAsSpectator;
        }

        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetRealityManager(this);
            OnRealityManagerSpawned?.Invoke(this);
        }

        private void OnConnectedAsSpectator()
        {
            StartScanningQRCode();
        }

        private void StartScanningQRCode()
        {
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>(true);
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            arTrackedImageManager.enabled = true;
        }

        // TODO: Design a better localization algorithm
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var image in args.added)
            {
                _imageStablizationFrameCount = 0;
            }

            foreach (var image in args.updated)
            {
                if (_imageStablizationFrameCount == 0)
                {
                    _lastImagePosition = image.transform.position;
                    _lastImageRotation = image.transform.rotation;
                }

                if (Vector3.Distance(image.transform.position, _lastImagePosition) < ImageStablizationPositionThreshold &&
                    Quaternion.Angle(image.transform.rotation, _lastImageRotation) < ImageStablizationRotationThreshold)
                {
                    _imageStablizationFrameCount++;
                    Debug.Log($"[RealityManager] image stablization frame count {_imageStablizationFrameCount}");
                    if (_imageStablizationFrameCount == ImageStabilizationFrameNum)
                    {

                    }
                }
                else
                {
                    _imageStablizationFrameCount = 0;
                }
            }
        }

        private void StopScanningQRCode()
        {
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            arTrackedImageManager.enabled = false;
        }


    }
}