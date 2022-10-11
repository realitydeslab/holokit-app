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
        public Vector3 ServerImagePosition;
        public Vector3 ClientImagePosition;
    }

    public struct CalibrationResult
    {
        public Vector3 Translate;
        public float Theta;
    }

    public class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        [SerializeField] private NetworkHostCameraPose _networkHostCameraPosePrefab;

        [SerializeField] private GameObject _phoneAlignmentMarkPrefab;

        private NetworkHostCameraPose _networkHostCameraPose;

        private GameObject _phoneAlignmentMark;

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
                _phoneAlignmentMark.transform.SetPositionAndRotation(_networkHostCameraPose.transform.position,
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
                HoloKitARSessionControllerAPI.RegisterARSessionUpdatedFrameDelegate();
                MultipeerConnectivityTransport.StartAdvertising();
            }
            SpawnNetworkHostCameraPose();
        }

        public void StopAdvertising()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitARSessionControllerAPI.RegisterARSessionUpdatedFrameDelegate();
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

        private double _timestmapOffsetToServer;

        private readonly Queue<ServerTimedCameraPose> _serverTimedCameraPoseQueue = new();

        private const double ServerTimedCameraPoseDuration = 0.6;

        private double _lastClientFrameTimestamp;

        private const double MaxTimestampDiff = 0.035; // For 30 FPS

        private readonly Queue<ImagePositionPair> _clientImagePositionPairQueue = new();

        private const int ClientImagePositionPairQueueStablizationCount = 10;

        private readonly Queue<CalibrationResult> _clientCalibrationResultQueue = new();

        private const int ClientCalibrationResultQueueStablizationCount = 10;

        private const double ClientThetaQueueStablizationStandardDeviationThreshold = 0.1 * Math.PI / 180.0; // In radians

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
                //Debug.Log($"[RealityManager] new timestamp offset standard deviation: {standardDeviation}");
                if (standardDeviation < TimestampOffsetQueueStablizationStandardDeviationThreshold)
                {
                    // This deviation is acceptable
                    _timestmapOffsetToServer = _timestampOffsetQueue.Average();
                    _currentClientSyncPhase = ClientSyncPhase.TimestampSynced;
                    Debug.Log($"[RealityManager] Timestamp synced with offset {_timestmapOffsetToServer}");
                }
                _timestampOffsetQueue.Dequeue();
            }
        }

        // Step 2: Start to use image tracking to scan the host's QR code
        private void StartScanningQRCode()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnClientARSessionUpdatedFrame;
            HoloKitARSessionControllerAPI.RegisterARSessionUpdatedFrameDelegate();
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>(true);
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            arTrackedImageManager.enabled = true;
        }

        private void StopScanningQRCode()
        {
            HoloKitARSessionControllerAPI.RegisterARSessionUpdatedFrameDelegate();
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnClientARSessionUpdatedFrame;
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            arTrackedImageManager.enabled = false;
        }

        private void OnServerARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            //Debug.Log($"-------------------");
            //Debug.Log($"[RealityManager] new Matrix:");
            //Debug.Log($"[RealityManager] {matrix[0, 0]} {matrix[0, 1]} {matrix[0, 2]} {matrix[0, 3]}\n" +
            //    $"{matrix[1, 0]} {matrix[1, 1]} {matrix[1, 2]} {matrix[1, 3]}\n" +
            //    $"{matrix[2, 0]} {matrix[2, 1]} {matrix[2, 2]} {matrix[2, 3]}\n" +
            //    $"{matrix[3, 0]} {matrix[3, 1]} {matrix[3, 2]} {matrix[3, 3]}");
            //Debug.Log($"[RealityManager] Unity matrix:");
            //Matrix4x4 unityMatrix = HoloKitCamera.Instance.CenterEyePose.localToWorldMatrix;
            //Debug.Log($"[RealityManager] {unityMatrix[0, 0]} {unityMatrix[0, 1]} {unityMatrix[0, 2]} {unityMatrix[0, 3]}\n" +
            //    $"{unityMatrix[1, 0]} {unityMatrix[1, 1]} {unityMatrix[1, 2]} {unityMatrix[1, 3]}\n" +
            //    $"{unityMatrix[2, 0]} {unityMatrix[2, 1]} {unityMatrix[2, 2]} {unityMatrix[2, 3]}\n" +
            //    $"{unityMatrix[3, 0]} {unityMatrix[3, 1]} {unityMatrix[3, 2]} {unityMatrix[3, 3]}");
            //Debug.Log($"[RealityManager] owner matrix position: {matrix.GetPosition()} and rotation: {matrix.rotation.eulerAngles}");
            //Debug.Log($"[RealityManager] camera position: {HoloKitCamera.Instance.CenterEyePose.position} and rotation: {HoloKitCamera.Instance.CenterEyePose.rotation.eulerAngles}");
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
                OnRequestServerImagePositionServerRpc(_lastClientFrameTimestamp + _timestmapOffsetToServer, image.transform.position);
            }
        }

        // Step 3: When scanned host's QRCode, request host's image position at that timestamp
        [ServerRpc(RequireOwnership = false)]
        private void OnRequestServerImagePositionServerRpc(double requestedServerTimestamp, Vector3 clientImagePosition, ServerRpcParams serverRpcParams = default)
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
                //Debug.Log($"[RealityManager] No! server pose not close enough, pose time: {nearestTimedCameraPose.Timestamp}, requested time: {requestedServerTimestamp}");
                return;
            }

            // Calculate image position using camera position and offset
            Vector3 serverImagePosition = nearestTimedCameraPose.PoseMatrix.GetPosition() +
                nearestTimedCameraPose.PoseMatrix.rotation * CameraToQRCodeOffset;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            OnReponseServerImagePositionClientRpc(nearestTimedCameraPose.PoseMatrix.GetPosition(), clientImagePosition, clientRpcParams);
        }

        [ClientRpc]
        private void OnReponseServerImagePositionClientRpc(Vector3 serverImagePosition, Vector3 clientImagePosition, ClientRpcParams clientRpcParams = default)
        {
            if (_currentClientSyncPhase != ClientSyncPhase.ScanningQRCode) { return; }

            //Debug.Log($"[Reality] Got image position pair server {serverImagePosition} and client {clientImagePosition}");
            _clientImagePositionPairQueue.Enqueue(new ImagePositionPair()
            {
                ServerImagePosition = serverImagePosition,
                ClientImagePosition = clientImagePosition
            });
            if (_clientImagePositionPairQueue.Count > ClientImagePositionPairQueueStablizationCount)
            {
                var calibrationResult = CalculateClientToServerTransform();
                _clientCalibrationResultQueue.Enqueue(calibrationResult);
                if (_clientCalibrationResultQueue.Count > ClientCalibrationResultQueueStablizationCount)
                {
                    var thetaStandardDeviation = HoloKitAppUtils.CalculateStdDev(_clientCalibrationResultQueue.Select(x => (double)x.Theta));
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

            string str = "";
            //Debug.Log($"[RealityManager] clientImagePositionCenter: {clientImagePositionCenter:F4} and serverImagePositionCenter: {serverImagePositionCenter:F4}");
            foreach (var imagePositionPair in _clientImagePositionPairQueue)
            {
                str += $"clientImagePosition: {imagePositionPair.ClientImagePosition:F4}, serverImagePosition: {imagePositionPair.ServerImagePosition:F4}, offset: {imagePositionPair.ClientImagePosition - imagePositionPair.ServerImagePosition:F4}\n";
            }
            Debug.Log(str);

            Vector2 tanThetaAB = _clientImagePositionPairQueue.Select(o =>
            {
                var p = o.ClientImagePosition - clientImagePositionCenter;
                var q = o.ServerImagePosition - serverImagePositionCenter;
                var a = p.x * q.x + p.z * q.z;
                var b = p.z * q.x - p.x * q.z;
                return new Vector2(a, b);
            }).Aggregate(Vector2.zero, (r, o) => r + o);

            float theta = (float)Math.Atan2(tanThetaAB.y, tanThetaAB.x);

            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(theta, new Vector3(0, 1, 0)));

            Vector3 translate = serverImagePositionCenter - rotation.MultiplyPoint3x4(clientImagePositionCenter);

            return new CalibrationResult()
            {
                Translate = translate,
                Theta = theta
            };
        }

        // Step 4: 
        private void OnClientCalibrationSucceeded()
        {
            _currentClientSyncPhase = ClientSyncPhase.PoseSynced;

            // Reset ARSession origin
            var lastCalibrationResult = _clientCalibrationResultQueue.Last();
            float theta = lastCalibrationResult.Theta;
            Vector3 translate = lastCalibrationResult.Translate;
            HoloKitARSessionControllerAPI.ResetOrigin(translate, Quaternion.AngleAxis(theta, new Vector3(0f, 1f, 0f)));

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
