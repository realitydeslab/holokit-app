using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public struct ImagePosePair
    {
        public Vector3 HostImagePosition; // QRCode image pose in host's coordinate system
        public Quaternion HostImageRotation;
        public Vector3 ClientImagePosition; // QRCode image pose in client's coordinate system
        public Quaternion ClientImageRotation;
    }

    // Rotate client's coordinate system by theta first, then translate
    public struct ClientSyncResult
    {
        public float ThetaInDeg; // Client to server
        public Vector3 Translate;
    }

    /// <summary>
    /// This is the client part of this partial class.
    /// </summary>
    public partial class HoloKitAppMultiplayerManager
    {
        /// <summary>
        /// The prefab of the phone alignment marker.
        /// </summary>
        [SerializeField] private GameObject _phoneAlignmentMarkerPrefab;

        /// <summary>
        /// This queue stores a sequence of timestamp offsets from the local 
        /// device to the host.
        /// </summary>
        private readonly Queue<double> _timestampOffsetQueue = new();

        /// <summary>
        /// We start to compute the stardard deviation of the timestamp offset
        /// queue when the size of the queue exceeds this number.
        /// </summary>
        private const int TimestampOffsetQueueStablizationCount = 10;

        /// <summary>
        /// The standard deviation of the timestamp offset queue must be less
        /// than this threshold.
        /// </summary>
        private const double TimestampOffsetQueueStandardDeviationThreshold = 0.01;

        /// <summary>
        /// The final result derived from the timestamp sync algorithm.
        /// </summary>
        private double _finalTimestampOffset;

        /// <summary>
        /// The timestamp of the lastest ARSession frame.
        /// </summary>
        private double _latestARSessionFrameTimestamp;

        /// <summary>
        /// Stores pairs of host image poses and client image poses.
        /// </summary>
        private readonly Queue<ImagePosePair> _imagePosePairQueue = new();

        /// <summary>
        /// We need at least this amount of image pairs to start the calculation.
        /// </summary>
        private const int ClientImagePosePairQueueStablizationCount = 50;

        // A constant whose unit from Cos(Angle) to m meter
        private const float OptimizationPenaltyConstant = 10;

        /// <summary>
        /// Stores a list of sync result.
        /// </summary>
        private readonly Queue<ClientSyncResult> _syncResultQueue = new();

        /// <summary>
        /// Start to compute the standard deviation of the queue when the number of elements
        /// in the queue exceeds this number.
        /// </summary>
        private const int ClientSyncResultQueueStablizationCount = 30;

        private const double ThetaStandardDeviationThreshold = 0.1; // In degrees

        /// <summary>
        /// The reference of the phone alignment marker.
        /// </summary>
        private GameObject _phoneAlignmentMarker;

        /// <summary>
        /// This event is called when the client checks the alignment marker.
        /// </summary>
        public static event Action OnAlignmentMarkerChecked;

        /// <summary>
        /// In Phase 1, we calculate the local device's timestamp offset to the host. 
        /// </summary>
        private void StartSpatialAnchorSyncProcess()
        {
            if (HoloKitUtils.IsRuntime)
            {
                _timestampOffsetQueue.Clear();
                _imagePosePairQueue.Clear();
                _syncResultQueue.Clear();
                _localPlayer.SyncStatus = HoloKitAppPlayerSyncStatus.SyncingTimestamp;
            }
            else
            {
                _localPlayer.SyncStatus = HoloKitAppPlayerSyncStatus.Synced;
            }
        }

        private void FixedUpdate()
        {
            // In this phase, client constantly request timestamp from the server.
            if (_localPlayer != null && _localPlayer.SyncStatus == HoloKitAppPlayerSyncStatus.SyncingTimestamp)
            {
                OnRequestTimestampServerRpc(HoloKitARSessionControllerAPI.GetSystemUptime());
            }
        }

        /// <summary>
        /// Receives the timestamp from the host.
        /// </summary>
        /// <param name="hostTimestamp"></param>
        /// <param name="oldClientTimestamp"></param>
        /// <param name="clientRpcParams"></param>
        [ClientRpc]
        private void OnRespondTimestampClientRpc(double hostTimestamp, double oldClientTimestamp, ClientRpcParams clientRpcParams = default)
        {
            if (_localPlayer.SyncStatus != HoloKitAppPlayerSyncStatus.SyncingTimestamp) { return; }

            double currentClientTimestamp = HoloKitARSessionControllerAPI.GetSystemUptime();
            double offset = hostTimestamp + (currentClientTimestamp - oldClientTimestamp) / 2 - currentClientTimestamp;
            _timestampOffsetQueue.Enqueue(offset);
            // We compute the stardard deviation if the size of the queue is greater
            // than the predefined number.
            if (_timestampOffsetQueue.Count > TimestampOffsetQueueStablizationCount)
            {
                double standardDeviation = HoloKitAppUtils.CalculateStdDev(_timestampOffsetQueue);
                if (standardDeviation < TimestampOffsetQueueStandardDeviationThreshold)
                {
                    // The stardard deviation of the current queue is acceptable
                    // Take the avarage value as the timestamp offset
                    _finalTimestampOffset = _timestampOffsetQueue.Average();
                    Debug.Log($"[SpatialAnchorSynchronization] Final timestamp offset: {_finalTimestampOffset}");
                    _localPlayer.SyncStatus = HoloKitAppPlayerSyncStatus.SyncingPose;
                    //StartScanningQRCode();
                }
                else
                {
                    Debug.Log($"[MultiplayerManager] The current standard deivation {standardDeviation} is too large, try again...");
                }
                _timestampOffsetQueue.Dequeue();
            }
        }

        /// <summary>
        /// In Phase 2, we use image tracking to scan the QR code on host's screen.
        /// </summary>
        private void StartScanningQRCode()
        {
            var arSessionManager = HoloKitApp.Instance.ARSessionManager;
            arSessionManager.ARTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame_Client;
            arSessionManager.SetARTrackedImageManagerEnabled(true);
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            if (args.updated.Count == 1)
            {
                var image = args.updated[0];
                // We only use the images with nice tracking quality.
                // https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/features/image-tracking.html#tracking-state
                if (image.trackingState == TrackingState.Tracking)
                {
                    OnRequestImagePoseServerRpc(_latestARSessionFrameTimestamp + _finalTimestampOffset, image.transform.position, image.transform.rotation);
                }
            }
            else if (args.updated.Count > 1)
            {
                Debug.Log("[MultiplayerManager] There are more than 1 QR code images detected, which is weird.");
            }
        }

        private void OnARSessionUpdatedFrame_Client(double timestamp, Matrix4x4 _)
        {
            _latestARSessionFrameTimestamp = timestamp;
        }

        [ClientRpc]
        private void OnRespondImagePoseClientRpc(Vector3 hostImagePosition, Quaternion hostImageRotation, Vector3 clientImagePosition, Quaternion clientImageRotation, ClientRpcParams clientRpcParams = default)
        {
            if (_localPlayer.SyncStatus != HoloKitAppPlayerSyncStatus.SyncingPose)  return;

            _imagePosePairQueue.Enqueue(new ImagePosePair()
            {
                HostImagePosition = hostImagePosition,
                HostImageRotation = hostImageRotation,
                ClientImagePosition = clientImagePosition,
                ClientImageRotation = clientImageRotation
            });
            // We wait until the queue reaches the minimal size.
            if (_imagePosePairQueue.Count > ClientImagePosePairQueueStablizationCount)
            {
                var clientSyncResult = CalculateClientSyncResult();
                _syncResultQueue.Enqueue(clientSyncResult);
                if (_syncResultQueue.Count > ClientSyncResultQueueStablizationCount)
                {
                    var thetaStandardDeviation = HoloKitAppUtils.CalculateStdDev(_syncResultQueue.Select(x => (double)x.ThetaInDeg));
                    if (thetaStandardDeviation < ThetaStandardDeviationThreshold)
                    {
                        // Client sync succeeded
                        OnSynced();
                        return;
                    }
                    else
                    {
                        _ = _syncResultQueue.Dequeue();
                    }
                }
                _ = _imagePosePairQueue.Dequeue();
            }
        }

        /// <summary>
        /// Use the least square method to calculate the result.
        /// </summary>
        /// <returns></returns>
        private ClientSyncResult CalculateClientSyncResult()
        {
            var clientImagePositionCenter = new Vector3(
                _imagePosePairQueue.Select(o => o.ClientImagePosition.x).Average(),
                _imagePosePairQueue.Select(o => o.ClientImagePosition.y).Average(),
                _imagePosePairQueue.Select(o => o.ClientImagePosition.z).Average());

            var hostImagePositionCenter = new Vector3(
                _imagePosePairQueue.Select(o => o.HostImagePosition.x).Average(),
                _imagePosePairQueue.Select(o => o.HostImagePosition.y).Average(),
                _imagePosePairQueue.Select(o => o.HostImagePosition.z).Average());

            Vector2 tanThetaAB = _imagePosePairQueue.Select(o =>
            {
                var p = o.HostImagePosition - hostImagePositionCenter;
                var q = o.ClientImagePosition - clientImagePositionCenter;
                var r = Matrix4x4.Rotate(o.HostImageRotation).transpose * Matrix4x4.Rotate(o.ClientImageRotation);

                var a = p.x * q.x + p.z * q.z + OptimizationPenaltyConstant * (r.m00 + r.m22);
                var b = -p.x * q.z + p.z * q.x + OptimizationPenaltyConstant * (-r.m20 + r.m02);
                return new Vector2(a, b);
            }).Aggregate(Vector2.zero, (r, o) => r + o);

            float thetaInDeg = (float)Math.Atan2(tanThetaAB.y, tanThetaAB.x) / Mathf.Deg2Rad;

            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(thetaInDeg, Vector3.up));

            Vector3 translate = -rotation.MultiplyPoint3x4(hostImagePositionCenter - clientImagePositionCenter);

            return new ClientSyncResult()
            {
                ThetaInDeg = thetaInDeg,
                Translate = translate
            };
        }

        private void OnSynced()
        {
            _localPlayer.SyncStatus = HoloKitAppPlayerSyncStatus.Synced;

            // We use the last result in the queue to reset ARSession origin
            var lastSyncResult = _syncResultQueue.Last();
            float theta = lastSyncResult.ThetaInDeg;
            Vector3 translate = lastSyncResult.Translate;
            HoloKitARSessionControllerAPI.ResetOrigin(translate, Quaternion.AngleAxis(theta, Vector3.up));

            StopScanningQRCode();
            SpawnPhoneAlignmentMarker();
        }

        private void StopScanningQRCode()
        {
            var arSessionManager = HoloKitApp.Instance.ARSessionManager;
            arSessionManager.ARTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame_Client;
            arSessionManager.SetARTrackedImageManagerEnabled(false);
        }

        // We only need to spawn this on client machine locally
        private void SpawnPhoneAlignmentMarker()
        {
            if (_phoneAlignmentMarker == null)
            {
                _phoneAlignmentMarker = Instantiate(_phoneAlignmentMarkerPrefab, GetServerPlayer().transform);
                _phoneAlignmentMarker.transform.localPosition = HostCameraToScreenCenterOffset.Value;
            }
        }

        private void DestroyPhoneAlignmentMarker()
        {
            if (_phoneAlignmentMarker != null)
            {
                Destroy(_phoneAlignmentMarker);
            }
        }

        public void CheckAlignmentMarker()
        {
            DestroyPhoneAlignmentMarker();
            OnAlignmentMarkerChecked?.Invoke();
        }

        public void RescanQRCode()
        {
            DestroyPhoneAlignmentMarker();
            StartSpatialAnchorSyncProcess();
        }
    }
}
