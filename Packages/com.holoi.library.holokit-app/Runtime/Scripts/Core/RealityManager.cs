using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using Netcode.Transports.MultipeerConnectivity;

namespace Holoi.Library.HoloKitApp
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public string SceneName;

        public List<GameObject> NetworkPrefabs;

        [SerializeField] private GameObject _phoneAlignmentMarkPrefab;

        private bool _isAdvertising;

        private Dictionary<ulong, string> _connectedSpectatorDevices = new();

        private Vector3 _lastImagePosition;

        private Quaternion _lastImageRotation;

        private int _imageStablizationFrameCount = 0;

        private const float ImageStablizationPositionThreshold = 0.03f;

        private const float ImageStablizationRotationThreshold = 7;

        private const int ImageStabilizationFrameNum = 80;

        private readonly Vector3 QRCodeToCameraOffset = new(-0.02f, 0.1f, 0.01f);

        private NetworkVariable<Vector3> _hostCameraPosition = new(Vector3.zero, NetworkVariableReadPermission.Everyone);

        private NetworkVariable<Quaternion> _hostCameraRotation = new(Quaternion.identity, NetworkVariableReadPermission.Everyone);

        private GameObject _phoneAlignmentMark;

        public static event Action<RealityManager> OnRealityManagerSpawned;

        public static event Action<List<string>> OnSpectatorDeviceListUpdated;

        public static event Action OnFinishedScanningQRCode;

        public static event Action OnQRCodeStabilizationFailed;

        protected virtual void Awake()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator += OnConnectedAsSpectator;
        }

        public override void OnDestroy()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator -= OnConnectedAsSpectator;
            if (_isAdvertising)
            {
                StopAdvertising();
            }
        }

        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetRealityManager(this);
            OnRealityManagerSpawned?.Invoke(this);

            if (!IsServer)
            {
                OnSpectatorJoinedServerRpc(SystemInfo.deviceName);
            }
        }

        private void OnConnectedAsSpectator()
        {
            StartScanningQRCode();
        }

        public void StartAdvertising()
        {
            if (HoloKitHelper.IsRuntime)
            {
                NetworkManager.OnClientDisconnectCallback += OnSpectatorDisconnected;
                _isAdvertising = true;
                MultipeerConnectivityTransport.StartAdvertising();
            }
        }

        public void StopAdvertising()
        {
            if (HoloKitHelper.IsRuntime)
            {
                NetworkManager.OnClientDisconnectCallback -= OnSpectatorDisconnected;
                _isAdvertising = false;
                MultipeerConnectivityTransport.StopAdvertising();
            }
        }

        [ServerRpc]
        private void OnSpectatorJoinedServerRpc(string spectatorDeviceName, ServerRpcParams serverRpcParams = default)
        {
            _connectedSpectatorDevices.Add(serverRpcParams.Receive.SenderClientId, spectatorDeviceName);
            UpdateSpectatorDeviceList();
        }

        private void OnSpectatorDisconnected(ulong clientId)
        {
            if (_connectedSpectatorDevices.ContainsKey(clientId))
            {
                _connectedSpectatorDevices.Remove(clientId);
                UpdateSpectatorDeviceList();
            }
        }

        private void UpdateSpectatorDeviceList()
        {
            List<string> spectatorDeviceList = new();
            Debug.Log("[RealityManager] OnSpectatorDeviceListUpdated");
            foreach (var spectatorDevice in _connectedSpectatorDevices.Values)
            {
                Debug.Log($"[RealityManager] Device name: {spectatorDevice}");
                spectatorDeviceList.Add(spectatorDevice);
            }
            OnSpectatorDeviceListUpdated?.Invoke(spectatorDeviceList);
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
                Debug.Log("[RealityManager] QRCode image added");
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
                    //Debug.Log($"[RealityManager] image stablization frame count {_imageStablizationFrameCount}");
                    if (_imageStablizationFrameCount == ImageStabilizationFrameNum)
                    {
                        Quaternion localHostCameraRotation = Quaternion.Euler(90f, 0f, 0f) * image.transform.rotation;
                        Vector3 localHostCameraPosition = image.transform.position + localHostCameraRotation * QRCodeToCameraOffset;

                        Quaternion originRotation = Quaternion.Inverse(_hostCameraRotation.Value) * localHostCameraRotation;
                        Vector3 originPosition = localHostCameraPosition + originRotation * _hostCameraPosition.Value;
                        HoloKitARSessionControllerAPI.ResetOrigin(originPosition, HoloKitAppUtils.GetHorizontalRotation(originRotation));
                        StopScanningQRCode();
                        OnFinishedScanningQRCode?.Invoke();
                        SpawnPhoneAlignmentMark();
                        Debug.Log("[RealityManager] QRCode stabilization succeeded");
                    }
                }
                else
                {
                    _imageStablizationFrameCount = 0;
                    Debug.Log("[RealityManager] QRCode stabilization failed");
                    OnQRCodeStabilizationFailed?.Invoke();
                }
            }
        }

        private void StopScanningQRCode()
        {
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            arTrackedImageManager.enabled = false;
        }

        // We only need to spawn this on client machine locally
        private void SpawnPhoneAlignmentMark()
        {
            if (_phoneAlignmentMark == null)
            {
                _phoneAlignmentMark = Instantiate(_phoneAlignmentMarkPrefab);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (IsServer)
            {
                if (_isAdvertising)
                {
                    _hostCameraPosition.Value = HoloKitCamera.Instance.CenterEyePose.position;
                    _hostCameraRotation.Value = HoloKitCamera.Instance.CenterEyePose.rotation;
                }
            }
            else
            {
                if (_phoneAlignmentMark != null)
                {
                    _phoneAlignmentMark.transform.SetPositionAndRotation(_hostCameraPosition.Value, _hostCameraRotation.Value);
                }
            }
        }
    }
}