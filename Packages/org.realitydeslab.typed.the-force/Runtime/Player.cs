// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using HoloKit;

namespace RealityDesignLab.Typed.TheForce
{
    public class Player : MonoBehaviour
    {
        Transform _centerEye;
        [SerializeField] HandGestureManager _HGM;
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _magicCube;
        [SerializeField] Transform _indicator;

        public event Action OnCastSomthingAction;
        public event Action OnCastNothingAction;
        public event Action<PlayerState> OnSwitchState;

        public enum PlayerState
        {
            empty,
            filled
        }

        enum castState
        {
            nothing,
            magicCube
        }

        castState _castState = castState.nothing;

        public PlayerState _mcState = PlayerState.empty;

        private void OnEnable()
        {
            OnSwitchState += SetIndicator;
        }
        private void OnDisable()
        {
            OnSwitchState += SetIndicator;
        }
        private void Start()
        {
            _centerEye = HoloKitCameraManager.Instance.CenterEyePose;
        }

        private void Update()
        {
            var dir = (_handJoint.position - _centerEye.position).normalized;
            transform.LookAt(transform.position + dir);
            Debug.DrawRay(_centerEye.position, dir * 10f);

            switch (_mcState)
            {
                case PlayerState.empty:
                    int layerMaskAttractable = LayerMask.GetMask("TheForceLayer");
                    Ray ray = new Ray(_centerEye.position, dir);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 10.0f, layerMaskAttractable))
                    {
                        // cast on target object now
                        if(_castState == castState.nothing)
                        {
                            OnCastSomthingAction?.Invoke();
                            _castState = castState.magicCube;
                        }
                        

                        if (_HGM.CurrentGesture == HandGestureManager.Gesture.Fist)
                        {
                            _magicCube = hit.transform;
                            _magicCube.GetComponent<MagicCube>().OnAttracted();
                            _mcState = PlayerState.filled;
                        }
                    }
                    else
                    {
                        if (_castState == castState.magicCube)
                        {
                            OnCastNothingAction?.Invoke();
                            _castState = castState.nothing;
                        }
                    }
                    break;
                case PlayerState.filled:
                    if (_HGM.CurrentGesture == HandGestureManager.Gesture.Palm)
                    {
                        _magicCube.GetComponent<MagicCube>().OnShooted(dir);
                        _mcState = PlayerState.empty;
                    }
                    break;
            }
        }


        void SetIndicator(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.empty:
                    _indicator.gameObject.SetActive(false);

                    break;
                case PlayerState.filled:
                    _indicator.gameObject.SetActive(true);

                    break;
            }
        }
    }

}
