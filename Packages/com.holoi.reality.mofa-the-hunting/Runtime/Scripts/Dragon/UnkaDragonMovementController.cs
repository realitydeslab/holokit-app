using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Holoi.Reality.MOFATheHunting
{
    public class UnkaDragonMovementController : MonoBehaviour
    {
        Transform _chasingTarget;

        enum MovementControllerType
        {
            SelfControl,
            KeyboardControl
        }

        [SerializeField] MovementControllerType _controllerType;

        [SerializeField] Animator _animator;

        [SerializeField] float _maxForce = .1f;

        [SerializeField] float _maxAngularForce = 1f;

        [SerializeField] float _maxSpeed = 4f;

        [Header("Distance Control")]
        [SerializeField] float _avoidDistance = 2f;

        [SerializeField] float _attractDistance = 5f;

        Vector3 _velocity = new();
        float _angluarVelocity = new(); // only in y axis cause it's a flying vreature
        float _angularSteer = new();
        Vector3 _steer = new();
        Vector3 _acceleration = new();
        Vector3 Rotation = new();

        // debug
        Vector3 realDesired;

        void Start()
        {
            _chasingTarget = HoloKit.HoloKitCamera.Instance.CenterEyePose;
        }

        private void FixedUpdate()
        {
            Arrive(_chasingTarget.position);
            VelocityUpdate();
            RotationUpdate();
            AnimationUpdate();
        }

        void VelocityUpdate()
        {
            _velocity += _acceleration;

            _velocity = Limit(_velocity, _maxSpeed);

            //Debug.DrawRay(transform.position, _velocity / Time.deltaTime * 10f, Color.green);

            transform.position += _velocity * Time.deltaTime;

            //Debug.Log($"transform.position: {transform.position}");
            //Debug.Log($"_velocity: {_velocity}");
            //Debug.Log($"_acceleration: {_acceleration}");

            _acceleration = Vector3.zero;
        }

        void applyForce(Vector3 force)
        {
            _acceleration += force;
        }

        void Arrive(Vector3 targetPos)
        {
            switch (_controllerType)
            {
                case MovementControllerType.SelfControl:
                    realDesired = new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(transform.position.x, 0, transform.position.z);

                    float realDist = realDesired.magnitude;

                    if (realDist < _avoidDistance + 1f && realDist > _avoidDistance - 1f)
                    {
                        realDesired = Vector3.zero;
                    }
                    else if (realDist < _avoidDistance)
                    {
                        float m = (1f - (realDist / _avoidDistance)) * _maxSpeed;

                        //Debug.Log("avoid");
                        //        Debug.Log($"avoid with a d: {realDist}; " +
                        //$"with a m: {1 - (realDist / _avoidDistance)}; " +
                        //$"with a force: {realDesired}");

                        realDesired = SetMag(-realDesired, m);
                    }
                    else if (realDist < _attractDistance)
                    {
                        //Debug.Log("attract");

                        float m = ((realDist - _avoidDistance) / (_attractDistance - _avoidDistance)) * _maxSpeed;

                        realDesired = SetMag(realDesired, m);
                    }
                    else
                    {
                        realDesired = SetMag(realDesired, _maxSpeed);
                    }

                    break;
                case MovementControllerType.KeyboardControl:
                    UpdateGamepad();
                    break;
            }

            var dH = transform.position.y - (targetPos.y + 0.5f);

            var heightDesired = dH > 0 ? -1f : 1f;

            float mHeight = Mathf.Clamp01(Mathf.Abs(dH));


            realDesired += Vector3.up * heightDesired * mHeight;

            //Debug.Log($"realDesired: {realDesired}");

            _steer = realDesired - _velocity;

            _steer = Limit(_steer, _maxForce);

            //Debug.Log($"_steer: {_steer}");

            applyForce(_steer);

            realDesired = Vector3.zero;
        }

        void RotationUpdate()
        {
            var desiredDirection = (new Vector3(_chasingTarget.position.x, 0, _chasingTarget.position.z)
                - new Vector3(transform.position.x, 0, transform.position.z)).normalized;

            var signedAngle = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);

            var angle = Mathf.Abs(signedAngle);

            var m = 1f;

            if (angle < 90f)
            {
                m = angle / 90f;
            }
            else
            {
                m = 1;
            }

            _angularSteer = (signedAngle - _angluarVelocity) * m;

            _angularSteer = Mathf.Clamp(_angularSteer, -_maxAngularForce, _maxAngularForce);

            var angularAcceleration = _angularSteer;

            if (signedAngle > 0)
            {
                //Debug.Log($">0 with: {angle}");
                _angluarVelocity += angularAcceleration;
                if (_angluarVelocity > 90) _angluarVelocity = 90;
                Rotation += Vector3.up * _angluarVelocity * Time.deltaTime;
            }
            else
            {
                //Debug.Log($"<0 with: {angle}");
                _angluarVelocity += angularAcceleration;
                if (_angluarVelocity < -90) _angluarVelocity = -90;
                Rotation += Vector3.up * _angluarVelocity * Time.deltaTime;
            }

            transform.rotation = Quaternion.Euler(Rotation + new Vector3(0, 180, 0));

            //transform.LookAt(transform.position + _velocity);
        }

        void AnimationUpdate()
        {
            var velocity = transform.InverseTransformVector(_velocity);

            //Debug.DrawRay(transform.position, velocity.normalized + velocity / Time.deltaTime, Color.blue);

            //_animator.SetFloat("Velocity X", (_velocity.x / _maxSpeed) + (_angularSteer));

            _animator.SetFloat("Velocity X", (_velocity.x / _maxSpeed) + Mathf.Abs(_angluarVelocity) / 90f);

            _animator.SetFloat("Velocity Z", -_velocity.z / _maxSpeed);
        }

        Vector3 Limit(Vector3 velocity, float length)
        {
            if (velocity.magnitude > length)
            {
                //Debug.Log($"reach max with a value: {velocity.magnitude} higher than: {length}");
                return velocity.normalized * length;
            }
            else
            {
                return velocity;
            }
        }

        Vector3 SetMag(Vector3 velocity, float length)
        {
            return velocity.normalized * length;
        }

        void UpdateGamepad()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
            {
                //Debug.Log("not find pad");
                return; // No gamepad connected.
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                realDesired += transform.forward * _maxSpeed;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                realDesired -= transform.forward * _maxSpeed * 0.5f;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                realDesired -= transform.right * _maxSpeed;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                realDesired += transform.right * _maxSpeed;
            }

            if (gamepad.yButton.isPressed)
            {
                realDesired += transform.forward * _maxSpeed;
            }
            if (gamepad.xButton.isPressed)
            {
                realDesired -= transform.right * _maxSpeed;
            }

            if (gamepad.aButton.isPressed)
            {
                realDesired -= transform.forward * _maxSpeed;
            }
            if (gamepad.bButton.isPressed)
            {
                realDesired += transform.right * _maxSpeed;
            }

            if (gamepad.leftShoulder.wasPressedThisFrame)
            {
                // fire breath
                FindObjectOfType<UnkaDragonController>().FireBall = true;
            }

            if (gamepad.rightShoulder.wasPressedThisFrame)
            {
                // fire ball
                FindObjectOfType<UnkaDragonController>().FireBreath = true;
            }

            //Debug.Log(realDesired);

            if (gamepad.dpad.IsPressed())
            {
                //Debug.Log(gamepad.dpad.ReadValue());
            }
        }
    }
}