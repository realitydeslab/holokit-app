using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using BoidsSimulationOnGPU;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace Holoi.Reality.Typography
{
    public class BoidTypoSceneManager : MonoBehaviour
    {
        [SerializeField] GPUBoids _boids;
        [SerializeField] VisualEffect _vfx;
        // raycast
        ARRaycastManager _raycastManager;
        Pose _placementPose;
        bool placementPoseIsValid = false;
        float _directDistance = 0 ;
        float _verticalDistance = 0;
        //
        Transform centereye;
        // temp test
        [SerializeField] Camera _cam;
        [SerializeField] Transform hitPoseSample;

        private void Start()
        {
            _raycastManager = GetComponent<ARRaycastManager>();
             centereye = HoloKitCamera.Instance.CenterEyePose;

        }

        void Update()
        {
            SetVfxBuffer();
            UpdatePlacementPose();
        }

        
        private void UpdatePlacementPose()
        {
            Vector3 screenCenter = new Vector3(0,0,0);
            if (_cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)) != null)
            {
                screenCenter = _cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            }

            var hits = new List<ARRaycastHit>();

            if (_raycastManager != null)
            {
                _raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
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
                _directDistance = Vector3.Distance(centereye.position, _placementPose.position);

                var eyeOnXZ = new Vector2(centereye.position.x, centereye.position.z);
                var poseOnXZ = new Vector3(_placementPose.position.x, _placementPose.position.z);
                _verticalDistance = Vector2.Distance(eyeOnXZ, poseOnXZ);

                var offsetZ = 2f;

                if (_verticalDistance > 2.5f)
                {
                    offsetZ = 2f;
                }
                else
                {
                    offsetZ = ((_verticalDistance - 0.5f) / _verticalDistance) * _directDistance;
                }

                _boids.GetComponent<FollowMovementManager>().Offset =
                    new Vector3(0, 0, offsetZ);

                //Debug.Log("Did Hit with _directDistance: " + _directDistance);
            }

            /*
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;

            RaycastHit hit;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(centereye.position, GetHorizontalForward(centereye), out hit, Mathf.Infinity, layerMask))
            {
                _verticalDistance = Vector3.Distance(centereye.position, hit.point);

                Debug.Log("Did Hit with _verticalDistance: " + _verticalDistance);

                var offsetZ = 2f;

                if (_verticalDistance > 2.5f)
                {
                    offsetZ = 2f;
                }
                else
                {
                    offsetZ = ((_verticalDistance - 0.5f) / _verticalDistance) * _directDistance;
                }

                _boids.GetComponent<FollowMovementManager>().Offset =
                    new Vector3(0,0, offsetZ);
            }
            */
        }

        public static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }

        void SetVfxBuffer()
        {
            _vfx.SetGraphicsBuffer("PositionDataBuffer", _boids.GetBoidPositionDataBuffer());
            _vfx.SetGraphicsBuffer("VelocityDataBuffer", _boids.GetBoidVelocityDataBuffer());
        }
    }
}
