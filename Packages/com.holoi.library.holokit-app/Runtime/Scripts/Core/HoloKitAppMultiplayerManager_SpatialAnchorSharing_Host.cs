using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.MultipeerConnectivity;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public struct TimedCameraPose
    {
        public double Timestamp;
        public Matrix4x4 PoseMatrix;
    }

    /// <summary>
    /// This is the host part of this partial class.
    /// </summary>
    public partial class HoloKitAppMultiplayerManager
    {
        /// <summary>
        /// The offset from the host's camera to its QRCode center on the screen.
        /// </summary>
        [HideInInspector] public readonly NetworkVariable<Vector3> HostCameraToQRCodeOffset = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The offset from the host's camera to its screen center. This is used by the client to
        /// correctly render the alignmenr marker.
        /// </summary>
        [HideInInspector] public readonly NetworkVariable<Vector3> HostCameraToScreenCenterOffset = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public bool IsAdvertising => _isAdvertising;

        /// <summary>
        /// Whether the master is currently advertising?
        /// </summary>
        private bool _isAdvertising = false;

        /// <summary>
        /// Stores a queue of camera poses with timestamp on the host.
        /// </summary>
        private readonly Queue<TimedCameraPose> _hostTimedCameraPoseQueue = new();

        /// <summary>
        /// Host does not store poses with timestamp early than this value. TODO: Adjust this value
        /// </summary>
        private const double HostTimedCameraPoseTimestampThreshold = 0.6;

        /// <summary>
        /// We only process camera poses with timestamp deviation smaller than this value.
        /// </summary>
        private const double TimestampDeviationThreshold = 0.034; // For 30 FPS

        public void StartAdvertising()
        {
            SetHostCameraToScreenCenterOffset();
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame_Host;
            MultipeerConnectivityTransport.StartAdvertising();
            _isAdvertising = true;
        }

        public void StopAdvertising()
        {
            if (_isAdvertising)
            {
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame_Host;
                MultipeerConnectivityTransport.StopAdvertising();
                _hostTimedCameraPoseQueue.Clear();
                _isAdvertising = false;
            }
        }

        /// <summary>
        /// Calculate the offset from the host's camera to its screen center.
        /// This function is host only, make sure you are in portrait mode when calling this method.
        /// </summary>
        private void SetHostCameraToScreenCenterOffset()
        {
            if (HoloKitUtils.IsEditor) return;

            Vector3 phoneModelCameraOffset = HoloKitOpticsAPI.GetPhoneModelCameraOffset(HoloKitType.HoloKitX);
            Vector3 portraitCameraOffset = new(phoneModelCameraOffset.y, -phoneModelCameraOffset.x, phoneModelCameraOffset.z);
            float halfScreenWidthInMeter = HoloKitUtils.PixelToMeter(Screen.width / 2f);
            HostCameraToScreenCenterOffset.Value = new Vector3(portraitCameraOffset.x + halfScreenWidthInMeter,
                                                                 portraitCameraOffset.y,
                                                                 portraitCameraOffset.z);
        }

        private void OnARSessionUpdatedFrame_Host(double timestamp, Matrix4x4 matrix)
        {
            TimedCameraPose timedCameraPose = new()
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

        /// <summary>
        /// Request the host to send back its timestamp.
        /// </summary>
        /// <param name="clientTimestamp"></param>
        /// <param name="serverRpcParams"></param>
        [ServerRpc(RequireOwnership = false)]
        private void OnRequestTimestampServerRpc(double clientTimestamp, ServerRpcParams serverRpcParams = default)
        {
            // Get the current server timestamp
            double hostTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            OnRespondTimestampClientRpc(hostTimestamp, clientTimestamp, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnRequestImagePoseServerRpc(double requestedTimestamp, Vector3 clientImagePosition, Quaternion clientImageRotation, ServerRpcParams serverRpcParams = default)
        {
            if (_hostTimedCameraPoseQueue.Count == 0) return;

            double minTimestampDeviation = 99;
            TimedCameraPose nearestTimedCameraPose = new();
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
                nearestTimedCameraPose.PoseMatrix.rotation * HostCameraToQRCodeOffset.Value;
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
    }
}
