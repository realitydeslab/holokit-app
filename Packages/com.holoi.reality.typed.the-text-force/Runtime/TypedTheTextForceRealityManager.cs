// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections;
using UnityEngine;
using HoloKit;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typed.TheTextForce
{
    public class TypedTheTextForceRealityManager : MonoBehaviour
    {
        public event Action OnCastCubeAction;

        public event Action OnNotCastCubeAction;

        public enum State
        {
            holdEmpty,
            holdCube
        }

        public State _state = State.holdEmpty;

        [SerializeField] Transform _centerEye;

        [SerializeField] HandGestureManager _handGestureManager;

        [SerializeField] GameObject _prefabForceCube;

        [SerializeField] GameObject _prefabForceObject;

        TheTextForceCubeController _magicCube;

        int _objectMaxCount = 4;

        int _currentObjectCount = 0;

        int _cubeMaxCount = 1;

        int _currentCubeCount = 0;

        private void Start()
        {
            _centerEye = HoloKitCameraManager.Instance.CenterEyePose;
            StartCoroutine(WaitAndCreateMagicObjectAndCube());
        }

        private void Update()
        {
            var raycastDirection = (HandObject.Instance.transform.position - _centerEye.position).normalized;

            //transform.LookAt(transform.position + raycastDirection);

            if(HoloKitUtils.IsEditor)
            Debug.DrawRay(_centerEye.position, raycastDirection * 10f, Color.red);

            switch (_state)
            {
                case State.holdEmpty:

                    Ray ray = new Ray(_centerEye.position, raycastDirection);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 10.0f))
                    {
                        if (hit.transform.GetComponent<TheTextForceCubeController>())
                        {
                            _magicCube = hit.transform.GetComponent<TheTextForceCubeController>();
                            Debug.Log("cast on magic cube.");
                            OnCastCubeAction?.Invoke();

                            if (_handGestureManager.CurrentGesture == HandGestureManager.Gesture.Fist)
                            {
                                _magicCube.OnBeAttracted();
                                _state = State.holdCube;
                            }
                        }
                        else
                        {
                            Debug.Log("not cast on magic cube");
                            _magicCube = null;
                        }
                    }
                    else
                    {
                        Debug.Log("not cast on magic cube");
                        OnNotCastCubeAction?.Invoke();
                        _magicCube = null;
                    }
                    break;
                case State.holdCube:

                    if (_handGestureManager.CurrentGesture == HandGestureManager.Gesture.Palm)
                    {
                        _magicCube.GetComponent<TheTextForceCubeController>().OnBeShoot(raycastDirection);
                        _state = State.holdEmpty;
                    }

                    break;
            }
        }

        void CreateMagicObjectOnDetectedFloor()
        {
            LayerMask lm = 1 << 6;
            Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), -1, UnityEngine.Random.Range(0f, 1f));
            Ray ray = new Ray(_centerEye.position, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3.0f, lm))
            {
                CreateMagicObjct(hit.point);
            }
        }

        void CreateMagicCube()
        {
            _currentCubeCount++;
            if (_currentCubeCount! > _cubeMaxCount)
            {
                var go = Instantiate(_prefabForceCube);
                go.transform.position = _centerEye.position + _centerEye.forward;
                go.transform.GetComponent<FollowMovementManager>().FollowTarget = HandObject.Instance.transform;
            }
            else
            {
                Debug.Log("Cubes Number Reach Limitation, No More Cube.");
            }
        }
        void CreateMagicObjct(Vector3 position)
        {
            _currentObjectCount++;
            if (_currentObjectCount! > _objectMaxCount)
            {
                var go = Instantiate(_prefabForceObject);
                go.transform.position = position + Vector3.up * 0.5f;
            }
            else
            {
                Debug.Log("Objects Number Reach Limitation, No More Objects.");
            }
        }

        IEnumerator WaitAndCreateMagicObjectAndCube()
        {
            yield return new WaitForSeconds(4f);
            CreateMagicCube();
            CreateMagicObjectOnDetectedFloor();
        }
    }
}
