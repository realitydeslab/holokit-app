using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using Netcode.Transports.MultipeerConnectivity;

namespace Holoi.Library.HoloKitApp
{
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

    public enum ClientSyncPhase
    {
        NotStarted = 0,
        SyncingTimestamp = 1,
        TimestampSynced = 2,
        ScanningQRCode = 3,
        PoseSynced = 4
    }

    public abstract class RealityManager : NetworkBehaviour
    {
        public string SceneName;

        public List<GameObject> NetworkPrefabs;

        private bool _isAdvertising;

        private readonly Dictionary<ulong, string> _connectedSpectatorDevices = new();

        private Vector3 _lastImagePosition;

        private Quaternion _lastImageRotation;

        private int _imageStablizationFrameCount = 0;

        private const float ImageStablizationPositionThreshold = 0.03f;

        private const float ImageStablizationRotationThreshold = 7;

        private const int ImageStabilizationFrameNum = 80;

        private readonly Vector3 QRCodeToCameraOffset = new(-0.02f, 0.1f, 0.01f); // TODO: Compute this

        private readonly NetworkVariable<Vector3> _hostCameraPosition = new(Vector3.zero, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<Quaternion> _hostCameraRotation = new(Quaternion.identity, NetworkVariableReadPermission.Everyone);

        private GameObject _phoneAlignmentMark;

        #region Sync
        private Queue<double> _timestampOffsetQueue = new();

        private const int TimestampOffsetQueueStablizationThreshold = 10;

        private const double TimestampOffsetQueueStablizationStandardDeviation = 0.01;

        private double _timestmapOffsetToServer;

        private ClientSyncPhase _currentClientSyncPhase;

        private Queue<ServerTimedCameraPose> _serverTimedCameraPoseQueue = new();

        private const double ServerTimedCameraPoseExpirationDuration = 0.6;

        private double _lastClientFrameTimestamp;

        private Queue<ImagePositionPair> _clientImagePositionPairQueue = new();

        private const double MaxTimestampDiff = 0.04;

        private const int ClientImagePositionPairQueueThreshold = 10;

        private Queue<CalibrationResult> _calibrationResultQueue = new();

        private const int CalibrationResultThreshold = 10;

        private const double CalibrationThetaStablizationStandardDeviation = 0.1 * Math.PI / 180.0; // In radians
        #endregion

        // OnLocalClientConnected
        public static event Action OnRealityManagerSpawned;

        public static event Action<List<string>> OnSpectatorDeviceListUpdated;

        public static event Action OnFinishedScanningQRCode;

        public static event Action OnQRCodeStabilizationFailed;

        protected virtual void Awake()
        {

        }

        public override void OnDestroy()
        {
            if (_isAdvertising)
            {
                StopAdvertising();
            }
        }

        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetRealityManager(this);
            OnRealityManagerSpawned?.Invoke();
        }

        public void StartAdvertising()
        {
            if (HoloKitHelper.IsRuntime)
            {
                NetworkManager.OnClientDisconnectCallback += OnSpectatorDisconnected;
                _isAdvertising = true;
                _serverTimedCameraPoseQueue.Clear();
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnServerARSessionUpdatedFrame;
                MultipeerConnectivityTransport.StartAdvertising();
            }
        }

        public void StopAdvertising()
        {
            if (HoloKitHelper.IsRuntime)
            {
                NetworkManager.OnClientDisconnectCallback -= OnSpectatorDisconnected;
                _isAdvertising = false;
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnServerARSessionUpdatedFrame;
                MultipeerConnectivityTransport.StopAdvertising();
            }
        }

        private void StartClientCalibration()
        {
            _timestampOffsetQueue.Clear();
            _clientImagePositionPairQueue.Clear();
            _calibrationResultQueue.Clear();

            _currentClientSyncPhase = ClientSyncPhase.SyncingTimestamp;
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
            while (_serverTimedCameraPoseQueue.Count > 0 && currentTime - _serverTimedCameraPoseQueue.Peek().Timestamp > ServerTimedCameraPoseExpirationDuration)
            {
                _ = _serverTimedCameraPoseQueue.Dequeue();
            }
        }

        [ServerRpc]
        private void OnRequestServerImagePositionServerRpc(double requestedServerTimestamp, Vector3 clientImagePosition, ServerRpcParams serverRpcParams = default)
        {
            if (_serverTimedCameraPoseQueue.Count == 0)
            {
                return;
            }

            double minTimestampDiff = 100;
            Vector3 nearestImagePosition = Vector3.zero;
            foreach (var timedCameraPose in _serverTimedCameraPoseQueue)
            {
                double timeDiff = Math.Abs(timedCameraPose.Timestamp - requestedServerTimestamp);
                if (timeDiff < minTimestampDiff)
                {
                    minTimestampDiff = timeDiff;
                    nearestImagePosition = timedCameraPose.PoseMatrix.GetPosition() + timedCameraPose.PoseMatrix.rotation * QRCodeToCameraOffset;
                }
            }

            if (minTimestampDiff > MaxTimestampDiff)
            {
                return;
            }

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };

            OnReponseServerImagePositionClientRpc(nearestImagePosition, clientImagePosition, clientRpcParams);
        }

        [ClientRpc]
        private void OnReponseServerImagePositionClientRpc(Vector3 serverImagePosition, Vector3 clientImagePosition, ClientRpcParams clientRpcParams = default)
        {
            _clientImagePositionPairQueue.Enqueue(new ImagePositionPair()
            {
                ServerImagePosition = serverImagePosition,
                ClientImagePosition = clientImagePosition
            });
            if (_clientImagePositionPairQueue.Count > ClientImagePositionPairQueueThreshold)
            {
                var calibrationResult = CalculateClientToServerTransform();
                _calibrationResultQueue.Enqueue(calibrationResult);
                if (_calibrationResultQueue.Count > CalibrationResultThreshold)
                {
                    var deviation = CalculateStdDev(_calibrationResultQueue.Select(x => (double)x.Theta));
                    if (deviation < CalibrationThetaStablizationStandardDeviation)
                    {
                        // Calibration succeeded!
                        OnClientCalibrationSucceeded();
                    }

                    _ = _calibrationResultQueue.Dequeue();
                }

                _ = _clientImagePositionPairQueue.Dequeue();
            }
        }

        private void OnClientCalibrationSucceeded()
        {
            _currentClientSyncPhase = ClientSyncPhase.PoseSynced;

            var lastCalibrationResult = _calibrationResultQueue.Last();
            float theta = lastCalibrationResult.Theta;
            Vector3 translate = lastCalibrationResult.Translate;
            HoloKitARSessionControllerAPI.ResetOrigin(translate, Quaternion.AngleAxis(theta, new Vector3(0f, 1f, 0f)));

            StopScanningQRCode();
            OnFinishedScanningQRCode?.Invoke();
            SpawnPhoneAlignmentMark();
            OnSpectatorJoinedServerRpc(SystemInfo.deviceName);
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
                var p = o.ClientImagePosition - clientImagePositionCenter;
                var q = o.ServerImagePosition - serverImagePositionCenter;
                var a = p.x * q.x + p.z * q.z;
                var b = p.z * q.x - p.x * q.z;
                return new Vector2(a, b);
            }).Aggregate(Vector2.zero, (r, o) => r + o);

            float theta = (float)Math.Atan2(tanThetaAB.y, tanThetaAB.x);

            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(theta, new Vector3(0, 1, 0)));

            Vector3 translate = serverImagePositionCenter - rotation.MultiplyPoint3x4(clientImagePositionCenter);

            return new CalibrationResult() {
                Translate = translate,
                Theta = theta
            };
        }

        // Step 1:
        [ServerRpc]
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
            double currentClientTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            double offset = serverTimestamp + (currentClientTimestamp - oldClientTimestamp) / 2 - currentClientTimestamp;
            _timestampOffsetQueue.Enqueue(offset);
            if (_timestampOffsetQueue.Count > TimestampOffsetQueueStablizationThreshold)
            {
                double deviation = CalculateStdDev(_timestampOffsetQueue);
                if (deviation < TimestampOffsetQueueStablizationStandardDeviation)
                {
                    // Confirm this deviation
                    _timestmapOffsetToServer = _timestampOffsetQueue.Average();
                    _currentClientSyncPhase = ClientSyncPhase.TimestampSynced;
                }

                _timestampOffsetQueue.Dequeue();
            }
        }

        private double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;

            if (values.Count() > 0)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                //Put it all together
                ret = Math.Sqrt((sum) / values.Count() - 1);
            }
            return ret;
        }

        protected virtual void FixedUpdate()
        {
            if (IsServer)
            {
                //if (_isAdvertising)
                //{
                //    //_hostCameraPosition.Value = HoloKitCamera.Instance.CenterEyePose.position;
                //    //_hostCameraRotation.Value = HoloKitCamera.Instance.CenterEyePose.rotation;
                //}
            }
            else
            {
                if (!IsSpawned) { return; }

                if (_currentClientSyncPhase == ClientSyncPhase.NotStarted)
                {
                    if (HoloKitHelper.IsRuntime)
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

                if (_phoneAlignmentMark != null)
                {
                    Debug.Log($"host camera pose {_hostCameraPosition.Value} {_hostCameraRotation.Value}");
                    _phoneAlignmentMark.transform.SetPositionAndRotation(_hostCameraPosition.Value, _hostCameraRotation.Value);
                }
            }
        }

        [ClientRpc]
        private void UpdateHostCameraPoseClientRpc(Vector3 position, Quaternion rotation, long timestamp)
        {
            Debug.Log($"[OnHostCameraPoseUpdated] position: {position}, rotation: {rotation} and timestamp: {timestamp}");
        }

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

        private void StartScanningQRCode()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnClientARSessionUpdatedFrame;
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>(true);
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            arTrackedImageManager.enabled = true;
        }

        private void StopScanningQRCode()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnClientARSessionUpdatedFrame;
            var arTrackedImageManager = HoloKitCamera.Instance.GetComponentInParent<ARTrackedImageManager>();
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            arTrackedImageManager.enabled = false;
        }

        private void OnClientARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            _lastClientFrameTimestamp = timestamp;
        }

        // AMBER TODO: Make a better localization algorithm
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            //foreach (var image in args.added)
            //{
            //    Debug.Log("[RealityManager] QRCode image added");
            //    _imageStablizationFrameCount = 0;
            //}

            foreach (var image in args.updated)
            {
                OnRequestServerImagePositionServerRpc(_lastClientFrameTimestamp + _timestmapOffsetToServer, image.transform.position);
                //if (_imageStablizationFrameCount == 0)
                //{
                //    _lastImagePosition = image.transform.position;
                //    _lastImageRotation = image.transform.rotation;
                //}

                //if (Vector3.Distance(image.transform.position, _lastImagePosition) < ImageStablizationPositionThreshold &&
                //    Quaternion.Angle(image.transform.rotation, _lastImageRotation) < ImageStablizationRotationThreshold)
                //{
                //    _imageStablizationFrameCount++;
                //    if (_imageStablizationFrameCount == ImageStabilizationFrameNum)
                //    {
                //        Debug.Log("[RealityManager] QRCode stabilization succeeded");
                //        GameObject go = new();
                //        go.transform.SetPositionAndRotation(image.transform.position, image.transform.rotation);
                //        go.transform.Rotate(go.transform.right, 90f);
                //        Vector3 localHostCameraPosition = go.transform.position + go.transform.rotation * QRCodeToCameraOffset;
                //        Quaternion localHostCameraRotation = go.transform.rotation;
                //        Destroy(go);

                //        Quaternion originRotation = Quaternion.Inverse(_hostCameraRotation.Value) * localHostCameraRotation;
                //        Vector3 originPosition = localHostCameraPosition + originRotation * -_hostCameraPosition.Value;
                //        //HoloKitARSessionControllerAPI.ResetOrigin(originPosition, originRotation);
                //        HoloKitARSessionControllerAPI.ResetOrigin(originPosition, HoloKitAppUtils.GetHorizontalRotation(originRotation));

                //        StopScanningQRCode();
                //        OnFinishedScanningQRCode?.Invoke();
                //        SpawnPhoneAlignmentMark();
                //        OnSpectatorJoinedServerRpc(SystemInfo.deviceName);
                //    }
                //}
                //else
                //{
                //    Debug.Log("[RealityManager] QRCode stabilization failed");
                //    _imageStablizationFrameCount = 0;
                //    OnQRCodeStabilizationFailed?.Invoke();
                //}
            }
        }

        // We only need to spawn this on client machine locally
        private void SpawnPhoneAlignmentMark()
        {
            if (_phoneAlignmentMark == null)
            {
                _phoneAlignmentMark = Instantiate(HoloKitApp.Instance.PhoneAlignmentMarkPrefab);
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
        }

        public void RescanQRCode()
        {
            DestroyPhoneAlignmentMark();
            //StartScanningQRCode();
            _currentClientSyncPhase = ClientSyncPhase.NotStarted;
        }
    }
}