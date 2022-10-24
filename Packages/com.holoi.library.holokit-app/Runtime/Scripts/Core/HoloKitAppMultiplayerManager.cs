using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Netcode.Transports.MultipeerConnectivity;
using UnityEngine.XR.ARFoundation;

namespace Holoi.Library.HoloKitApp
{
    public enum ClientSyncPhase
    {
        NotStarted = 0,
        SyncingTimestamp = 1,
        TimestampSynced = 2,
        ScanningQRCode = 3,
        PoseSynced = 4
    }

    public struct ServerTimedCameraPose
    {
        public double Timestamp;
        public Matrix4x4 PoseMatrix;
    }

    public struct ImagePositionPair
    {
        public Vector3 ServerImagePosition; // QRCode image pose in server's coordinate system
        public Quaternion ServerImageRotation;
        public Vector3 ClientImagePosition; // QRCode image pose in client's coordinate system
        public Quaternion ClientImageRotation;
    }

    // Rotate client's coordinate system by theta first, then translate
    public struct CalibrationResult
    {
        public float ThetaInDeg; // Client to server
        public Vector3 Translate;
    }

    public class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        [SerializeField] private NetworkHostCameraPose _networkHostCameraPosePrefab;

        [SerializeField] private GameObject _phoneAlignmentMarkPrefab;

        private NetworkHostCameraPose _networkHostCameraPose;

        private GameObject _phoneAlignmentMark;

        private readonly NetworkVariable<Vector3> _hostCameraToDisplayCenterOffset = new(Vector3.zero, NetworkVariableReadPermission.Everyone);

        private readonly Dictionary<ulong, string> _connectedSpectatorDevices = new();

        public static event Action OnLocalClientConnected;

        public static event Action<List<string>> OnSpectatorDeviceListUpdated;

        public static event Action OnFinishedScanningQRCode;

        public static event Action OnQRCodeStabilizationFailed;

        public static event Action OnAlignmentMarkChecked;

        #region Mono
        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetMultiplayerManager(this);
            OnLocalClientConnected?.Invoke();
        }

        private void Update()
        {
            if (_phoneAlignmentMark != null)
            {
                _phoneAlignmentMark.transform.SetPositionAndRotation(
                    _networkHostCameraPose.transform.position + _networkHostCameraPose.transform.rotation * _hostCameraToDisplayCenterOffset.Value,
                    _networkHostCameraPose.transform.rotation);
            }
        }

        private void FixedUpdate()
        {
            if (!IsServer)
            {
                if (!IsSpawned) { return; }

                if (_currentClientSyncPhase == ClientSyncPhase.NotStarted)
                {
                    if (HoloKitUtils.IsRuntime)
                    {
                        StartClientCalibration();
                    }
                    else
                    {
                        _currentClientSyncPhase = ClientSyncPhase.PoseSynced;
                    }
                }
                else if (_currentClientSyncPhase == ClientSyncPhase.SyncingTimestamp)
                {
                    OnTimeSyncRequestServerRpc(HoloKitARSessionControllerAPI.GetSystemUptime());
                }
                else if (_currentClientSyncPhase == ClientSyncPhase.TimestampSynced)
                {
                    _currentClientSyncPhase = ClientSyncPhase.ScanningQRCode;
                    StartScanningQRCode();
                }
            }
        }
        #endregion

        public void StartAdvertising()
        {
            if (HoloKitUtils.IsRuntime)
            {
                _serverTimedCameraPoseQueue.Clear();
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnServerARSessionUpdatedFrame;
                MultipeerConnectivityTransport.StartAdvertising();
            }
            SpawnNetworkHostCameraPose();
            UpdateCameraToDisplayCenterOffset();
        }

        public void StopAdvertising()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnServerARSessionUpdatedFrame;
                MultipeerConnectivityTransport.StopAdvertising();
            }
            DestroyNetworkHostCameraPose();
        }

        #region Pose sync
        public Vector3 CameraToQRCodeOffset
        {
            get => _cameraToQRCodeOffset;
            set
            {
                _cameraToQRCodeOffset = value;
            }
        }

        private Vector3 _cameraToQRCodeOffset;

        private ClientSyncPhase _currentClientSyncPhase;

        private readonly Queue<double> _timestampOffsetQueue = new();

        private const int TimestampOffsetQueueStablizationCount = 10;

        private const double TimestampOffsetQueueStablizationStandardDeviationThreshold = 0.01;

        private double _timestampOffsetToServer;

        private readonly Queue<ServerTimedCameraPose> _serverTimedCameraPoseQueue = new();

        private const double ServerTimedCameraPoseDuration = 0.6;

        private double _lastClientFrameTimestamp;

        private const double MaxTimestampDiff = 0.035; // For 30 FPS

        private Queue<ImagePositionPair> _clientImagePositionPairQueue = new();

        private const int ClientImagePositionPairQueueStablizationCount = 50;

        private readonly Queue<CalibrationResult> _clientCalibrationResultQueue = new();

        private const int ClientCalibrationResultQueueStablizationCount = 30;

        private const double ClientThetaQueueStablizationStandardDeviationThreshold = 0.1; // In degrees

        private const float OptimizationPenaltyConstant = 10; // A constant whose unit from Cos(Angle) to m meter. 


        // Step 1: Calculate timestamp offset between two devices
        private void StartClientCalibration()
        {
            _timestampOffsetQueue.Clear();
            _clientImagePositionPairQueue.Clear();
            _clientCalibrationResultQueue.Clear();

            _currentClientSyncPhase = ClientSyncPhase.SyncingTimestamp;
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnTimeSyncRequestServerRpc(double clientTimestamp, ServerRpcParams serverRpcParams = default)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            double serverTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            OnTimeSyncResponseClientRpc(serverTimestamp, clientTimestamp, clientRpcParams);
        }

        [ClientRpc]
        private void OnTimeSyncResponseClientRpc(double serverTimestamp, double oldClientTimestamp, ClientRpcParams clientRpcParams = default)
        {
            if (_currentClientSyncPhase != ClientSyncPhase.SyncingTimestamp) { return; }

            double currentClientTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            double offset = serverTimestamp + (currentClientTimestamp - oldClientTimestamp) / 2 - currentClientTimestamp;
            _timestampOffsetQueue.Enqueue(offset);
            if (_timestampOffsetQueue.Count > TimestampOffsetQueueStablizationCount)
            {
                double standardDeviation = HoloKitAppUtils.CalculateStdDev(_timestampOffsetQueue);
                if (standardDeviation < TimestampOffsetQueueStablizationStandardDeviationThreshold)
                {
                    // This deviation is acceptable
                    _timestampOffsetToServer = _timestampOffsetQueue.Average();
                    _currentClientSyncPhase = ClientSyncPhase.TimestampSynced;
                    Debug.Log($"[RealityManager] Timestamp synced with offset {_timestampOffsetToServer}");
                }
                _timestampOffsetQueue.Dequeue();
            }
        }

        // Step 2: Start to use image tracking to scan the host's QR code
        private void StartScanningQRCode()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnClientARSessionUpdatedFrame;
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>(true);
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            foreach (var trackable in arTrackedImageManager.trackables)
            {
                trackable.gameObject.SetActive(true);
            }
            arTrackedImageManager.enabled = true;
        }

        private void StopScanningQRCode()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnClientARSessionUpdatedFrame;
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            arTrackedImageManager.enabled = false;
            foreach (var trackable in arTrackedImageManager.trackables)
            {
                trackable.gameObject.SetActive(false);
            }
        }

        private void OnServerARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            ServerTimedCameraPose timedCameraPose = new ServerTimedCameraPose()
            {
                Timestamp = timestamp,
                PoseMatrix = matrix
            };
            _serverTimedCameraPoseQueue.Enqueue(timedCameraPose);
            double currentTime = HoloKitARSessionControllerAPI.GetSystemUptime();
            while (_serverTimedCameraPoseQueue.Count > 0 && currentTime - _serverTimedCameraPoseQueue.Peek().Timestamp > ServerTimedCameraPoseDuration)
            {
                _ = _serverTimedCameraPoseQueue.Dequeue();
            }
        }

        private void OnClientARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            _lastClientFrameTimestamp = timestamp;
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var image in args.updated)
            {
                OnRequestServerImagePositionServerRpc(_lastClientFrameTimestamp + _timestampOffsetToServer, image.transform.position, image.transform.rotation);
            }
        }

        // Step 3: When scanned host's QRCode, request host's image position at that timestamp
        [ServerRpc(RequireOwnership = false)]
        private void OnRequestServerImagePositionServerRpc(double requestedServerTimestamp, Vector3 clientImagePosition, Quaternion clientImageRotation, ServerRpcParams serverRpcParams = default)
        {
            if (_serverTimedCameraPoseQueue.Count == 0)
            {
                return;
            }

            double minTimestampDiff = 99;
            ServerTimedCameraPose nearestTimedCameraPose = new();
            foreach (var timedCameraPose in _serverTimedCameraPoseQueue)
            {
                double timeDiff = Math.Abs(timedCameraPose.Timestamp - requestedServerTimestamp);
                if (timeDiff < minTimestampDiff)
                {
                    minTimestampDiff = timeDiff;
                    nearestTimedCameraPose = timedCameraPose;
                }
            }

            if (minTimestampDiff > MaxTimestampDiff)
            {
                Debug.Log($"[RealityManager] No! server pose not close enough, pose time: {nearestTimedCameraPose.Timestamp}, requested time: {requestedServerTimestamp}");
                return;
            }

            // Calculate image position using camera position and offset
            Vector3 serverImagePosition = nearestTimedCameraPose.PoseMatrix.GetPosition() +
                nearestTimedCameraPose.PoseMatrix.rotation * CameraToQRCodeOffset;
            //Matrix4x4 serverImagePose = nearestTimedCameraPose.PoseMatrix * Matrix4x4.Translate(CameraToQRCodeOffset);
            Quaternion serverImageRotation = nearestTimedCameraPose.PoseMatrix.rotation;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            OnReponseServerImagePositionClientRpc(serverImagePosition, serverImageRotation, clientImagePosition, clientImageRotation, clientRpcParams);
        }

        [ClientRpc]
        private void OnReponseServerImagePositionClientRpc(Vector3 serverImagePosition, Quaternion serverImageRotation, Vector3 clientImagePosition, Quaternion clientImageRotation, ClientRpcParams clientRpcParams = default)
        {
            if (_currentClientSyncPhase != ClientSyncPhase.ScanningQRCode) { return; }

            //Debug.Log($"[Reality] Got image position pair server {serverImagePosition} and client {clientImagePosition}");
            _clientImagePositionPairQueue.Enqueue(new ImagePositionPair()
            {
                ServerImagePosition = serverImagePosition,
                ServerImageRotation = serverImageRotation,
                ClientImagePosition = clientImagePosition,
                ClientImageRotation = clientImageRotation
            });
            if (_clientImagePositionPairQueue.Count > ClientImagePositionPairQueueStablizationCount)
            {
                var calibrationResult = CalculateClientToServerTransform();
                //Debug.Log($"[Calibration] theta: {calibrationResult.ThetaInDeg}, translate: {calibrationResult.Translate}");
                _clientCalibrationResultQueue.Enqueue(calibrationResult);
                if (_clientCalibrationResultQueue.Count > ClientCalibrationResultQueueStablizationCount)
                {
                    var thetaStandardDeviation = HoloKitAppUtils.CalculateStdDev(_clientCalibrationResultQueue.Select(x => (double)x.ThetaInDeg));
                    if (thetaStandardDeviation < ClientThetaQueueStablizationStandardDeviationThreshold)
                    {
                        Debug.Log($"[RealityManager] Yes! theta deviation {thetaStandardDeviation:F4}, threshold {ClientThetaQueueStablizationStandardDeviationThreshold:F4}");
                        // Calibration succeeded!
                        OnClientCalibrationSucceeded();
                    }
                    else
                    {
                        Debug.Log($"[RealityManager] No! theta deviation {thetaStandardDeviation:F4}, threshold {ClientThetaQueueStablizationStandardDeviationThreshold:F4}");
                    }
                    _ = _clientCalibrationResultQueue.Dequeue();
                }
                _ = _clientImagePositionPairQueue.Dequeue();
            }
        }

        private CalibrationResult CalculateClientToServerTransform()
        {
            var clientImagePositionCenter = new Vector3(
                _clientImagePositionPairQueue.Select(o => o.ClientImagePosition.x).Average(),
                _clientImagePositionPairQueue.Select(o => o.ClientImagePosition.y).Average(),
                _clientImagePositionPairQueue.Select(o => o.ClientImagePosition.z).Average());

            var serverImagePositionCenter = new Vector3(
                _clientImagePositionPairQueue.Select(o => o.ServerImagePosition.x).Average(),
                _clientImagePositionPairQueue.Select(o => o.ServerImagePosition.y).Average(),
                _clientImagePositionPairQueue.Select(o => o.ServerImagePosition.z).Average());

            Vector2 tanThetaAB = _clientImagePositionPairQueue.Select(o =>
            {
                var p = o.ServerImagePosition - serverImagePositionCenter;
                var q = o.ClientImagePosition - clientImagePositionCenter;
                var r = Matrix4x4.Rotate(o.ServerImageRotation).transpose * Matrix4x4.Rotate(o.ClientImageRotation);

                var a = p.x * q.x + p.z * q.z + OptimizationPenaltyConstant * (r.m00 + r.m22);
                var b = -p.x * q.z + p.z * q.x + OptimizationPenaltyConstant * (-r.m20 + r.m02);
                return new Vector2(a, b);
            }).Aggregate(Vector2.zero, (r, o) => r + o);

            float thetaInDeg = (float)Math.Atan2(tanThetaAB.y, tanThetaAB.x) / Mathf.Deg2Rad;

            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(thetaInDeg, Vector3.up));

            Vector3 translate =  -rotation.MultiplyPoint3x4(serverImagePositionCenter - clientImagePositionCenter);

            return new CalibrationResult()
            {
                Translate = translate,
                ThetaInDeg = thetaInDeg
            };
        }

        // Step 4: 
        private void OnClientCalibrationSucceeded()
        {
            _currentClientSyncPhase = ClientSyncPhase.PoseSynced;

            // Reset ARSession origin
            var lastCalibrationResult = _clientCalibrationResultQueue.Last();
            float theta = lastCalibrationResult.ThetaInDeg;
            Vector3 translate = lastCalibrationResult.Translate;
            HoloKitARSessionControllerAPI.ResetOrigin(translate, Quaternion.AngleAxis(theta, Vector3.up));

            StopScanningQRCode();
            OnFinishedScanningQRCode?.Invoke();
            SpawnPhoneAlignmentMark();
            OnSpectatorJoinedServerRpc(SystemInfo.deviceName);
        }
        #endregion

        [ServerRpc(RequireOwnership = false)]
        private void OnSpectatorJoinedServerRpc(string spectatorDeviceName, ServerRpcParams serverRpcParams = default)
        {
            if (!_connectedSpectatorDevices.ContainsKey(serverRpcParams.Receive.SenderClientId))
            {
                _connectedSpectatorDevices.Add(serverRpcParams.Receive.SenderClientId, spectatorDeviceName);
                UpdateSpectatorDeviceList();
            }
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

        private void SpawnNetworkHostCameraPose()
        {
            if (_networkHostCameraPose == null)
            {
                var instance = Instantiate(_networkHostCameraPosePrefab);
                instance.GetComponent<NetworkObject>().Spawn();
            }
        }

        public void SetNetworkHostCameraPose(NetworkHostCameraPose networkHostCameraPose)
        {
            _networkHostCameraPose = networkHostCameraPose;
            _networkHostCameraPose.transform.SetParent(transform);
        }

        private void DestroyNetworkHostCameraPose()
        {
            if (_networkHostCameraPose != null)
            {
                Destroy(_networkHostCameraPose.gameObject);
            }
        }

        // Host only, make sure you are in portrait mode when calling this method.
        private void UpdateCameraToDisplayCenterOffset()
        {
            Vector3 phoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 portraitCameraOffset = new(phoneModelCameraOffset.y, -phoneModelCameraOffset.x, phoneModelCameraOffset.z);
            float halfScreenWidthInMeter = HoloKitUtils.PixelToMeter(Screen.width / 2f);
            _hostCameraToDisplayCenterOffset.Value = new Vector3(portraitCameraOffset.x + halfScreenWidthInMeter,
                                                                 portraitCameraOffset.y,
                                                                 portraitCameraOffset.z);
        }

        // We only need to spawn this on client machine locally
        private void SpawnPhoneAlignmentMark()
        {
            if (_phoneAlignmentMark == null)
            {
                _phoneAlignmentMark = Instantiate(_phoneAlignmentMarkPrefab);
            }
        }

        private void DestroyPhoneAlignmentMark()
        {
            if (_phoneAlignmentMark != null)
            {
                Destroy(_phoneAlignmentMark);
            }
        }

        public void CheckAlignmentMark()
        {
            DestroyPhoneAlignmentMark();
            OnAlignmentMarkChecked?.Invoke();
        }

        public void RescanQRCode()
        {
            DestroyPhoneAlignmentMark();
            //StartScanningQRCode();
            _currentClientSyncPhase = ClientSyncPhase.NotStarted;
        }
    }
}
