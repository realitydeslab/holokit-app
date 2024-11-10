// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typed.TheForce
{
    public class MagicCube : MonoBehaviour
    {
        public Transform MagicObject;

        [SerializeField] Player _player;
        [SerializeField] FollowMovementManager _FMM;
        [SerializeField] MeshRenderer _outlineMeshRenderer;
        //[SerializeField] float _attractDist = 1f;

        [SerializeField] GameObject _burstVfx;
        [SerializeField] GameObject _releaseVfx;

        [SerializeField] AudioSource _castAudio;
        [SerializeField] AudioSource _clickAudio;

        Material _mat;
        Animator _animator;
        //bool _isTriggered = false;

        [HideInInspector] public enum IntakeState
        {
            idle,
            needFilled,
            filled,
            needRelease,
            coolingDown
        }

        [HideInInspector] public enum MovementState
        {
            Free,
            Attracted
        }

        [HideInInspector] public IntakeState _intakeState = IntakeState.idle;
        [HideInInspector] public MovementState _movemenrtState = MovementState.Free;

        private void OnEnable()
        {
            _player = FindObjectOfType<Player>();
            _player.OnCastSomthingAction += OnCasted;
            _player.OnCastNothingAction += OnNotCasted;
        }

        private void OnDisable()
        {
            _player.OnCastSomthingAction -= OnCasted;
            _player.OnCastNothingAction -= OnNotCasted;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _mat = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if(transform.position.y < -2f)
            {
                transform.position = HoloKit.HoloKitCameraManager.Instance.CenterEyePose.position +
                    HoloKit.HoloKitCameraManager.Instance.CenterEyePose.forward;
            }

            if (MagicObject)
            {
                switch (_movemenrtState)
                {
                    case MovementState.Free:
                        switch (_intakeState)
                        {
                            case IntakeState.idle:
                                break;
                            case IntakeState.needFilled:
                                break;
                            case IntakeState.filled:
                                break;
                        }
                        break;
                    case MovementState.Attracted:
                        break;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (_intakeState)
            {
                case IntakeState.needFilled:
                    if(collision.transform.gameObject.layer != 7)
                    OnIntake();
                    break;
                case IntakeState.needRelease:
                    OnRelease();
                    break;
            }
        }


        public void OnIntake()
        {
            _animator.SetTrigger("Intake");
            MagicObject.GetComponent<MagicObject>().BeIntaken();
            StartCoroutine(WaitAndSetStateToFilled());

        }

        IEnumerator WaitAndSetStateToFilled()
        {
            yield return new WaitForSeconds(1f);
            _intakeState = IntakeState.filled;
        }

        public void OnRelease()
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * 1;
            _intakeState = IntakeState.idle;
            _animator.SetTrigger("Release");

            MagicObject.GetChild(0).gameObject.SetActive(true);
            MagicObject.position = transform.position;
            MagicObject.GetComponent<Rigidbody>().velocity = Vector3.up * 1;
            MagicObject.GetComponent<Animator>().SetTrigger("Release");
            MagicObject.GetComponent<MagicObject>().BeReleased();
        }

        public void OnAttracted()
        {
            if(_intakeState == IntakeState.needFilled)
            {
                _intakeState = IntakeState.idle; // hand gesture is not 100% accurate so we set idle again to decrease the times trigger un-expect event
            }
            if (_intakeState == IntakeState.needRelease)
            {
                _intakeState = IntakeState.filled;
            }

            SetStateToAttracted();

            _clickAudio.Play();
        }


        public void OnShooted(Vector3 direcion)
        {
            if(_intakeState == IntakeState.idle)
            {
                _intakeState = IntakeState.needFilled;
            }
            
            if (_intakeState == IntakeState.filled)
            {
                _intakeState = IntakeState.needRelease;
            }

            SetStateToIdle();
            transform.GetComponent<Rigidbody>().velocity = direcion * 3f;
        }

        public void SetStateToAttracted()
        {
            OnNotCasted();
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Rigidbody>().useGravity = false;
            _movemenrtState = MovementState.Attracted;
            _FMM.enabled = true;
        }
        public void SetStateToIdle()
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Rigidbody>().useGravity = true;
            _movemenrtState = MovementState.Free;
            _FMM.enabled = false;
        }

        public void OnCasted()
        {
            _outlineMeshRenderer.material.SetFloat("_Alpha_Multipier", 1);
            _castAudio.Play();
        }

        public void OnNotCasted()
        {
            _outlineMeshRenderer.material.SetFloat("_Alpha_Multipier", 0);
        }

        public void OnIntakeBurst()
        {
            var go = Instantiate(_burstVfx);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }

        public void OnReleaseBurst()
        {
            var go = Instantiate(_releaseVfx);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }
    }
}
