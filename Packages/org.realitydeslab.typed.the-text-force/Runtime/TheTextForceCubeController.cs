// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityDesignLab.Library.ARUX;

namespace RealityDesignLab.Typed.TheTextForce
{
    public class TheTextForceCubeController : MonoBehaviour
    {
        TypedTheTextForceRealityManager _manager;

        [SerializeField] FollowMovementManager _followManager;

        [SerializeField] MeshRenderer _outerMeshRenderer;

        [SerializeField] float _attractDist = 1f;

        [SerializeField] GameObject _intakeVfxPrefab;

        [SerializeField] GameObject _releaseVfxPrefab;

        TheTextForceObjectController _magicObject;

        Animator _animator;

        bool _isFilled = false;

        [HideInInspector]
        public enum IntakeState
        {
            idle,
            needFilled,
            filled,
            needRelease
        }

        [HideInInspector]
        public enum MovementState
        {
            Free,
            Attracted
        }

        [HideInInspector] public IntakeState _intakeState = IntakeState.idle;
        [HideInInspector] public MovementState _movemenrtState = MovementState.Free;

        private void OnEnable()
        {
            _manager = FindObjectOfType<TypedTheTextForceRealityManager>();
            _manager.OnCastCubeAction += OnCasted;
            _manager.OnNotCastCubeAction += OnNotCasted;
        }

        private void OnDisable()
        {
            _manager.OnCastCubeAction -= OnCasted;
            _manager.OnNotCastCubeAction -= OnNotCasted;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            
            if (_isFilled)
            {
                OnRelease(_magicObject);
  
            }
            else
            {
                if (collision.transform.GetComponent<TheTextForceObjectController>())
                {
                    _magicObject = collision.transform.GetComponent<TheTextForceObjectController>();
                    OnIntake(_magicObject);
                }
            }
        }

        void OnIntake(TheTextForceObjectController forceObject)
        {
            _isFilled = true;
            _intakeState = IntakeState.filled;
            _animator.SetTrigger("Intake");

            forceObject.BeIntaken();
        }

        void OnRelease(TheTextForceObjectController forceObject)
        {
            _isFilled = false;
            GetComponent<Rigidbody>().velocity = Vector3.up * 1;
            _intakeState = IntakeState.idle;
            _animator.SetTrigger("Release");

            forceObject.BeReleased();
        }

        public void OnBeAttracted()
        {
            if (_intakeState == IntakeState.needFilled)
            {
                _intakeState = IntakeState.idle; // hand gesture is not 100% accurate so we set idle again to decrease the times trigger un-expect event
            }

            transform.GetComponent<Rigidbody>().useGravity = false;
            _movemenrtState = MovementState.Attracted;
            _followManager.enabled = true;
        }

        public void OnBeShoot(Vector3 direcion)
        {
            if (_intakeState == IntakeState.idle)
            {
                Debug.Log("state to needFilled");

                _intakeState = IntakeState.needFilled;
            }

            if (_intakeState == IntakeState.filled)
            {
                Debug.Log("state to needRelease");
                _intakeState = IntakeState.needRelease;
            }

            transform.GetComponent<Rigidbody>().useGravity = true;
            _movemenrtState = MovementState.Free;
            _followManager.enabled = false;
            transform.GetComponent<Rigidbody>().velocity = direcion * 3f;
        }


        public void OnCasted()
        {
            // when player cast ray on cube, the cube change visual as a tip
            //_meshRenderer.material.SetFloat("_Alpha_Multipier", 1);
        }

        public void OnNotCasted()
        {
            // when player not cast ray on cube, the cube change visual as a tip
            //_meshRenderer.material.SetFloat("_Alpha_Multipier", 0);
        }

        public void OnIntakeBrustVFXSpawn()
        {
            var go = Instantiate(_intakeVfxPrefab);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }

        public void OnReleaseBrustVFXSpawn()
        {
            var go = Instantiate(_releaseVfxPrefab);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }
    }
}
