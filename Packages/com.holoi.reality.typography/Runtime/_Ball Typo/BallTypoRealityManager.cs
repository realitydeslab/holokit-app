using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Holoi.Reality.Typography
{
    public class BallTypoRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] Transform _handFollower;
        [SerializeField] Transform _ball;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public enum State
        {
            idle,
            handsUp,
            shoot,
            free
        }

        public State _state = State.idle;

        private void Start()
        {
            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;

        }

        void Update()
        {
            switch (_state)
            {
                case State.idle:

                    break;
                case State.handsUp:
                    _ball.GetComponent<FollowMovementManager>().enabled = true;
                    _ball.GetComponent<Rigidbody>().useGravity = false;
                    UpdateHandFollowerPosition();
                    break;
                case State.shoot:
                    var direction = _centerEye.forward;

                    _ball.GetComponent<FollowMovementManager>().enabled = false;
                    _ball.GetComponent<Rigidbody>().useGravity = true;
                    _ball.GetComponent<Rigidbody>().velocity = direction * 3;

                    _state = State.free;
                    break;
                case State.free:

                    break;
            }
        }

        void UpdateHandFollowerPosition()
        {
            var offset = _centerEye.right * 0.55f + _centerEye.up * 0.5f;

            _handFollower.position = _centerEye.position + offset;
        }

        public void SwitchStateToHandsUp()
        {
            _state = State.handsUp;
        }
        public void SwitchStateToShoot()
        {
            _state = State.shoot;
        }
    }
}