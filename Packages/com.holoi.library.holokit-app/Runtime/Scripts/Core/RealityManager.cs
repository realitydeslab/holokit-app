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
            arTrackedImageManager.enabled = true;
        }

        private void StopScanningQRCode()
        {
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.enabled = false;
        }
    }
}