using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.MultipeerConnectivity;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public enum DeviceStatus
    {
        Connecting = 0,
        Syncing = 1,
        Synced = 2
    }

    public struct DeviceInfo
    {
        public string Name;
        public DeviceStatus Status;
    }

    public struct HostTimedCameraPose
    {
        public double Timestamp;
        public Matrix4x4 PoseMatrix;
    }

    /// <summary>
    /// Host part of this partial class
    /// </summary>
    public partial class HoloKitAppMultiplayerManager
    {
        [SerializeField] private HostCameraPose _hostCameraPosePrefab;

        public List<DeviceInfo> ConnectedDevicesList => _connectedDevices.Values.ToList();

        public Vector3 HostCameraToQRCodeOffset
        {
            set
            {
                _hostCameraToQRCodeOffset = value;
            }
        }

        private HostCameraPose _hostCameraPose;

        /// <summary>
        /// Stores the connected devices' info on the host.
        /// </summary>
        private readonly Dictionary<ulong, DeviceInfo> _connectedDevices = new();

        /// <summary>
        /// Stores a queue of camera poses with timestamp on the host.
        /// </summary>
        private readonly Queue<HostTimedCameraPose> _hostTimedCameraPoseQueue = new();

        /// <summary>
        /// Host does not store poses with timestamp early than this value. TODO: Adjust this value
        /// </summary>
        private const double HostTimedCameraPoseTimestampThreshold = 0.6;

        /// <summary>
        /// We only process camera poses with timestamp deviation smaller than this value.
        /// </summary>
        private const double TimestampDeviationThreshold = 0.034; // For 30 FPS

        /// <summary>
        /// The offset from the host camera to the center of the QRCode on its screen.
        /// </summary>
        private Vector3 _hostCameraToQRCodeOffset;

        /// <summary>
        /// The offset from the host camera to its screen center. This is used by the client to
        /// correctly render an alignmenr marker.
        /// </summary>
        private readonly NetworkVariable<Vector3> _hostCameraToScreenCenterOffset = new(Vector3.zero, NetworkVariableReadPermission.Everyone);

        public static event Action OnAdvertisingStarted;

        public static event Action OnAdvertisingStopped;

        public static event Action<List<DeviceInfo>> OnConnectedDeviceListUpdated;

        private void Start_Host()
        {
            MultipeerConnectivityTransport.OnConnectedWithPeer += OnConnectedWithPeer_Host;
            MultipeerConnectivityTransport.OnDisconnectedWithPeer += OnDisconnectedWithPeer_Host;
        }

        private void OnDestroy_Host()
        {
            MultipeerConnectivityTransport.OnConnectedWithPeer -= OnConnectedWithPeer_Host;
            MultipeerConnectivityTransport.OnDisconnectedWithPeer -= OnDisconnectedWithPeer_Host;
        }

        public void StartAdvertising()
        {
            if (HoloKitUtils.IsEditor) return;

            SpawnHostCameraPose();
            SetHostCameraToScreenCenterOffset();
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame_Host;
            MultipeerConnectivityTransport.StartAdvertising();
            OnAdvertisingStarted?.Invoke();
        }

        private void SpawnHostCameraPose()
        {
            if (_hostCameraPose == null)
            {
                var instance = Instantiate(_hostCameraPosePrefab);
                instance.GetComponent<NetworkObject>().Spawn();
            }
        }

        public void SetHostCameraPose(HostCameraPose hostCameraPose)
        {
            _hostCameraPose = hostCameraPose;
        }

        private void DestroyHostCameraPose()
        {
            if (_hostCameraPose != null)
            {
                Destroy(_hostCameraPose.gameObject);
            }
        }

        // Host only, make sure you are in portrait mode when calling this method.
        private void SetHostCameraToScreenCenterOffset()
        {
            Vector3 phoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 portraitCameraOffset = new(phoneModelCameraOffset.y, -phoneModelCameraOffset.x, phoneModelCameraOffset.z);
            float halfScreenWidthInMeter = HoloKitUtils.PixelToMeter(Screen.width / 2f);
            _hostCameraToScreenCenterOffset.Value = new Vector3(portraitCameraOffset.x + halfScreenWidthInMeter,
                                                                 portraitCameraOffset.y,
                                                                 portraitCameraOffset.z);
        }

        private void OnARSessionUpdatedFrame_Host(double timestamp, Matrix4x4 matrix)
        {
            HostTimedCameraPose timedCameraPose = new()
            {
                Timestamp = timestamp,
                PoseMatrix = matrix
            };
            _hostTimedCameraPoseQueue.Enqueue(timedCameraPose);
            double currentTime = HoloKitARSessionControllerAPI.GetSystemUptime();
            // Get rid of early poses
            while (_hostTimedCameraPoseQueue.Count > 0 && currentTime - _hostTimedCameraPoseQueue.Peek().Timestamp > HostTimedCameraPoseTimestampThreshold)
            {
                _ = _hostTimedCameraPoseQueue.Dequeue();
            }
        }

        public void StopAdvertising()
        {
            if (HoloKitUtils.IsEditor) return;

            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame_Host;
            MultipeerConnectivityTransport.StopAdvertising();
            DestroyHostCameraPose();
            _hostTimedCameraPoseQueue.Clear();
            OnAdvertisingStopped?.Invoke();
        }

        private void OnConnectedWithPeer_Host(ulong clientId, string peerName)
        {
            if (IsServer)
            {
                DeviceInfo deviceInfo = new()
                {
                    Name = peerName,
                    Status = DeviceStatus.Syncing
                };

                if (_connectedDevices.ContainsKey(clientId))
                {
                    _connectedDevices[clientId] = deviceInfo;
                }
                else
                {
                    _connectedDevices.Add(clientId, deviceInfo);
                }
                OnConnectedDeviceListUpdated?.Invoke(_connectedDevices.Values.ToList());
            }
        }

        private void OnDisconnectedWithPeer_Host(ulong clientId, string peerName)
        {
            if (IsServer)
            {
                if (_connectedDevices.ContainsKey(clientId))
                {
                    _connectedDevices.Remove(clientId);
                    OnConnectedDeviceListUpdated?.Invoke(_connectedDevices.Values.ToList());
                }
            }
        }

        /// <summary>
        /// Requests the host to send back its timestamp.
        /// </summary>
        /// <param name="clientTimestamp"></param>
        /// <param name="serverRpcParams"></param>
        [ServerRpc(RequireOwnership = false)]
        private void OnRequestTimestampServerRpc(double clientTimestamp, ServerRpcParams serverRpcParams = default)
        {
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            double hostTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            OnRespondTimestampClientRpc(hostTimestamp, clientTimestamp, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnRequestImagePoseServerRpc(double requestedTimestamp, Vector3 clientImagePosition, Quaternion clientImageRotation, ServerRpcParams serverRpcParams = default)
        {
            if (_hostTimedCameraPoseQueue.Count == 0) return;

            double minTimestampDeviation = 99;
            HostTimedCameraPose nearestTimedCameraPose = new();
            // TODO: Optimize this enumeration
            foreach (var timedCameraPose in _hostTimedCameraPoseQueue)
            {
                double timestampDeviation = Math.Abs(timedCameraPose.Timestamp - requestedTimestamp);
                if (timestampDeviation < minTimestampDeviation)
                {
                    minTimestampDeviation = timestampDeviation;
                    nearestTimedCameraPose = timedCameraPose;
                }
            }

            // We don't accept camera pose with timestamp not close enough to the requested timestamp.
            if (minTimestampDeviation > TimestampDeviationThreshold) return;

            // Calculate the QRCode image position with offset
            Vector3 hostImagePosition = nearestTimedCameraPose.PoseMatrix.GetPosition() +
                nearestTimedCameraPose.PoseMatrix.rotation * _hostCameraToQRCodeOffset;
            // Get the correct rotation
            //Quaternion hostImageRotation = nearestTimedCameraPose.PoseMatrix.rotation;
            Quaternion hostImageRotation = nearestTimedCameraPose.PoseMatrix.rotation * Quaternion.Euler(-90f, 0f, 0f);
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            OnRespondImagePoseClientRpc(hostImagePosition, hostImageRotation, clientImagePosition, clientImageRotation, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnClientPoseSyncedServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            if (_connectedDevices.ContainsKey(clientId))
            {
                DeviceInfo deviceInfo = new()
                {
                    Name = _connectedDevices[clientId].Name,
                    Status = DeviceStatus.Synced
                };
                _connectedDevices[clientId] = deviceInfo;
                OnConnectedDeviceListUpdated?.Invoke(_connectedDevices.Values.ToList());
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnClientRescanServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            if (_connectedDevices.ContainsKey(clientId))
            {
                DeviceInfo deviceInfo = new()
                {
                    Name = _connectedDevices[clientId].Name,
                    Status = DeviceStatus.Syncing
                };
                _connectedDevices[clientId] = deviceInfo;
                OnConnectedDeviceListUpdated?.Invoke(_connectedDevices.Values.ToList());
            }
        }
    }
}
