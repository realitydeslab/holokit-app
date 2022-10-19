using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typography
{
    public class BallTypoRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;
        [SerializeField] Transform _serverHandFollower;
        [Header("Reality Objects")]
        [SerializeField] Transform _ball;
        [Header("Client UI")]
        [SerializeField] Button _handsUp;
        [SerializeField] Button _shoot;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public enum State
        {
            idle,
            handsUp,
            shoot,
            free
        }

        public State _state = State.idle;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                //_handsUp.gameObject.SetActive(false);
                //_shoot.gameObject.SetActive(false);
            }
            else
            {
                _handsUp.gameObject.SetActive(true);
                _shoot.gameObject.SetActive(true);
            }

            if (_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;

        }

        void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                UpdateHandFollowerPosition();
            }

            switch (_state)
            {
                case State.idle:

                    break;
                case State.handsUp:
                    Debug.Log("OnHandsUp");
                    _ball.GetComponent<BallController>().OnHandsUp();

                    break;
                case State.shoot:

                    var direction = _centerEye.forward;
                    _ball.GetComponent<BallController>().OnShoot(direction);
                    _state = State.free;
                    break;
                case State.free:

                    break;
            }
        }

        void UpdateHandFollowerPosition()
        {
            var offset = _centerEye.right * 0.55f + _centerEye.up * 0.5f;

            _serverHandFollower.position = _centerEye.position + offset;
        }

        public void OnHandsUpButtonClicked()
        {
            
            if (HoloKitApp.Instance.IsHost)
            {
                _state = State.handsUp;
            }
            else
            {
                // for server
                OnHandsUpButtonClickedServerRpc();
            }

        }

        public void OnShootButtonClicked()
        {
            
            if (HoloKitApp.Instance.IsHost)
            {
                _state = State.shoot;
            }
            else
            {
                // for server
                OnShootButtonClickedServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnHandsUpButtonClickedServerRpc()
        {
            Debug.Log("OnHandsUpButtonClickedServerRpc");

            _state = State.handsUp;

        }

        [ServerRpc(RequireOwnership = false)]
        private void OnShootButtonClickedServerRpc()
        {
            Debug.Log("OnShootButtonClickedServerRpc");
            _state = State.shoot;
        }
    }
}