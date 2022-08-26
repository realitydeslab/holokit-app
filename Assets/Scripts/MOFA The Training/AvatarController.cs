using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HoloKit;

public class AvatarController : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed;

    [SerializeField] private Animator _animator;

    private Vector3 _initialPosition;

    private Vector3 _initialForward;

    private Vector3 _initialRight;

    private Vector3 _targetPosition;

    private Vector3 _lastFramePosition;

    private Vector3 _lastFrameForward;

    private Vector3 _lastFrameRight;

    private Vector4 _velocityRemap = new Vector4(-.02f, .02f, -1f, 1f);

    private bool _notFirstAnimationFrame;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            _initialPosition = transform.position;
            _initialForward = transform.forward;
            _initialRight = transform.right;
            Debug.Log($"Initial position: {_initialPosition}, initial forward: {_initialForward} and initial right: {_initialRight}");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Rotation
            Vector3 p1 = new(HoloKitCamera.Instance.CenterEyePose.transform.position.x, 0f, HoloKitCamera.Instance.CenterEyePose.transform.position.z);
            Vector3 p2 = new(transform.position.x, 0f, transform.position.z);
            Quaternion lookAtRotation = Quaternion.LookRotation(p1 - p2);
            transform.rotation = lookAtRotation;

            // Position
            if (Vector3.Distance(transform.position, _targetPosition) > 0.05f)
            {
                transform.position += _movementSpeed * Runner.DeltaTime * (_targetPosition - transform.position).normalized;
            }
            else
            {
                PickNextTargetPosition();
            }
        }
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void PickNextTargetPosition()
    {
        float frontBack = Random.Range(-1.5f, 1f);
        float leftRight = Random.Range(-3f, 3f);
        _targetPosition = _initialPosition + _initialForward * frontBack + _initialRight * leftRight;
        Debug.Log($"Next target position: {_targetPosition}");
    }

    private void UpdateMovementAnimation()
    {
        if (_notFirstAnimationFrame)
        {
            // Calculate the relative z and x velocity
            Vector3 distFromLastFrame = transform.position - _lastFramePosition;
            float z = Vector3.Dot(distFromLastFrame, _lastFrameForward);
            float x = Vector3.Dot(distFromLastFrame, _lastFrameRight);

            var staticThreshold = 0.001667f; // if velocity < 0.1m/s, we regard it static.
            z = Utils.InverseClamp(z, -1 * staticThreshold, 1 * staticThreshold);
            x = Utils.InverseClamp(x, -1 * staticThreshold, 1 * staticThreshold);

            z = Utils.Remap(z, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);
            x = Utils.Remap(x, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);

            _animator.SetFloat("Velocity Z", z);
            _animator.SetFloat("Velocity X", x);
        }
        else
        {
            _notFirstAnimationFrame = true;
        }

        // Save data for next frame calculation
        _lastFrameForward = transform.forward;
        _lastFrameRight = transform.right;
        _lastFramePosition = transform.position;
    }
}
