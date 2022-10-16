using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using BoidsSimulationOnGPU;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;

namespace Holoi.Reality.Typography
{
    public class BoidTypoSceneManager : MonoBehaviour
    {
        [Header("Reality Objects")]
        [SerializeField] GameObject _boidPrefab;
        [SerializeField] GameObject _boid;
        GameObject _boidInstance;
        VisualEffect _vfx;
        [SerializeField] Transform _rotateAroundPlayer;
        [Header("AR Base Objects")]
        [SerializeField] Transform _serverCenterEye;

        // raycast
        ARRaycastManager _raycastManager;
        Pose _placementPose;
        bool placementPoseIsValid = false;
        float _directDistance = 0 ;
        float _verticalDistance = 0;
        //
        Transform _centereye;
        // temp test
        [SerializeField] Transform hitPoseSample;

        private void Start()
        {
            _raycastManager = GetComponent<ARRaycastManager>();
             _centereye = HoloKitCamera.Instance.CenterEyePose;

            _vfx = _boid.GetComponent<VisualEffect>();
            _vfx.enabled = true;

        }

        void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                UpdateServerCenterEye();
                //UpdateBoidCenterRotateAroundPlayer();
                //UpdateBoidCenter();
            }
            SetVfxBuffer();
        }

        void UpdateServerCenterEye()
        {
            _serverCenterEye.position = _centereye.position;
        }

        void UpdateBoidCenterRotateAroundPlayer()
        {
            if(_boid)
                _boid.transform.position = _rotateAroundPlayer.position;
        }

        /// <summary>
        /// this function devide how far the boid center from player.
        /// </summary>
        private void UpdateBoidCenter()
        {
            Vector3 horizontalForward = GetHorizontalForward(HoloKitCamera.Instance.CenterEyePose);

            Vector3 rayOrigin = _centereye.position;

            Ray ray = new(rayOrigin, _centereye.forward);

            var hits = new List<ARRaycastHit>();

            if (_raycastManager != null)
            {
                _raycastManager.Raycast(ray, hits, TrackableType.Planes);
            }
            else
            {
                Debug.Log("_raycastManager null!");
            }

            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                _placementPose = hits[0].pose;
                hitPoseSample.position = _placementPose.position;
                _directDistance = Vector3.Distance(_centereye.position, _placementPose.position);

                var eyeOnXZ = new Vector2(_centereye.position.x, _centereye.position.z);
                var poseOnXZ = new Vector3(_placementPose.position.x, _placementPose.position.z);
                _verticalDistance = Vector2.Distance(eyeOnXZ, poseOnXZ);

                var offsetZ = 2f;

                if (_verticalDistance > 2.5f)
                {
                    offsetZ = 2f;
                }
                else
                {
                    offsetZ = ((_verticalDistance - 0.25f - 0.5f) / _verticalDistance) * _directDistance; // o.5f is compersation of un-accurate
                }

                _boid.GetComponent<FollowMovementManager>().Offset =
                    new Vector3(0, 0, offsetZ);
            }
        }

        public static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }

        void SetVfxBuffer()
        {
            if (_vfx != null)
            {
                _vfx.SetGraphicsBuffer("PositionDataBuffer", _boid.GetComponent<GPUBoids>().GetBoidPositionDataBuffer());
                _vfx.SetGraphicsBuffer("VelocityDataBuffer", _boid.GetComponent<GPUBoids>().GetBoidVelocityDataBuffer());
            }
        }
    }
}
