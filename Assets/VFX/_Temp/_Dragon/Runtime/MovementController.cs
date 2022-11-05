using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [SerializeField] Transform _target;

    [SerializeField] float _maxForce = 1f;

    [SerializeField] float _maxSpeed = 1f;

    [Header("Distance Control")]
    [SerializeField] float _avoidDistance = 2f;
    [SerializeField] float _attractDistance = 5f;

    Vector3 _velocity = new();
    Vector3 _steer = new();
    Vector3 _lastVelocity = new();
    Vector3 _acceleration = new();

    // debug
    //Vector3 fixedTargetPosition;
    Vector3 realDesired;

    void Start()
    {
    }

    private void FixedUpdate()
    {
        Arrive(_target.position);
        VelocityUpdate();
        RotationUpdate();
        AnimationUpdate();
    }

    void VelocityUpdate()
    {
        _velocity += _acceleration;

        _velocity = Limit(_velocity, _maxSpeed);

        Debug.DrawRay(transform.position, _velocity / Time.deltaTime * 10f, Color.green);

        transform.position += _velocity;

        _acceleration = Vector3.zero;
    }

    void applyForce(Vector3 force)
    {
        // We could add mass here if we want A = F / M
        _acceleration += force;

        Debug.DrawRay(transform.position, _acceleration / Time.deltaTime * 10f, Color.blue);

    }

    void Arrive(Vector3 targetPos)
    {
        realDesired = new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(transform.position.x, 0, transform.position.z);

        float realDist = realDesired.magnitude;

        if (realDist < _avoidDistance)
        {
            float m = 1 - (realDist / _avoidDistance) * _maxSpeed;

            realDesired = SetMag(-realDesired, m);
        }
        else if (realDist < _attractDistance)
        {

            float m = ((realDist) / _attractDistance) * _maxSpeed;

            realDesired = SetMag(realDesired, m);
        }
        else
        {
            realDesired = SetMag(realDesired, _maxSpeed);
        }

        _steer = realDesired - _velocity;

        _steer = Limit(_steer, _maxForce);

        applyForce(_steer);
    }

    void RotationUpdate()
    {
        var realDirection = (new Vector3(_target.position.x, 0, _target.position.z)
            - new Vector3(transform.position.x, 0, transform.position.z)).normalized;

        var angle = Vector3.SignedAngle(realDirection, _velocity, Vector3.up);

        var angle2 = Vector3.SignedAngle(_velocity, transform.forward, Vector3.up);

        if (Mathf.Abs(angle2) > 90 * Time.deltaTime)
        {
            Debug.Log(" too much rotation");
            //transform.Rotate(0.0f, angle2 /Mathf.Abs(angle2) * 30 * Time.deltaTime, 0.0f, Space.World);
        }
        else
        {
            //transform.Rotate(0.0f, angle2 , 0.0f, Space.World);
            
        }

        

        if (Mathf.Abs(angle) > 90f)
        {
            transform.LookAt(transform.position + transform.forward  - _velocity);
        }
        else
        {
            transform.LookAt(transform.position + transform.forward + _velocity);
        }
    }

    void AnimationUpdate()
    {
        var velocity = transform.InverseTransformVector(_velocity);
        var steer = transform.InverseTransformVector(_steer);

        var realDesiredLocal = Limit(realDesired, _maxSpeed);
        realDesiredLocal = transform.InverseTransformVector(realDesiredLocal);

        //Debug.Log(velocity.x);
        //Debug.Log(velocity.z);

        //Debug.DrawRay(transform.position, velocity.normalized + velocity / Time.deltaTime, Color.blue);

        _animator.SetFloat("Velocity X", (realDesiredLocal.x * 100f) / _maxSpeed);

        _animator.SetFloat("Velocity Z", velocity.z / _maxSpeed);
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(fixedTargetPosition, 2);
    //}
}