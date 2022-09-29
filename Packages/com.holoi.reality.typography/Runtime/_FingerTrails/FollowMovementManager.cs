using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class FollowMovementManager : MonoBehaviour
{
    enum MovementType
    {
        HardFollow,
        SoftFollow,
        NotFollow
    }

    enum RotateType
    {
        InvertFacing,
        Facing,
        None,
        CopyRotation,
        FacingHorizentally
    }

    enum UpdateType
    {
        Always,
        OnlyBirth
    }

    [SerializeField] private Transform _followTarget;

    public Transform FollowTarget
    {
        set
        {
            _followTarget = value;
        }
    }

    [SerializeField] MovementType _movementType;
    [SerializeField] RotateType _rotateType;
    [SerializeField] UpdateType _updateType;

    [Header("Movement Properties")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, 0.5f);
    [SerializeField] private float _lerpSpeed = 4f;
    [SerializeField] private float _needMoveDistance = .01f;
    [SerializeField] private float _regardAsArriveDistance = .002f;

    private bool _needMove = false;

    public Vector3 Offset
    {
        set
        {
            _offset = value;
        }
        get
        {
            return _offset;
        }
    }

    Vector3 targetPosition;

    void Start()
    {
        if (!_followTarget) {_followTarget = HoloKitCamera.Instance.CenterEyePose; }
        else{}

        switch (_updateType)
        {
            case UpdateType.Always:

                break;
            case UpdateType.OnlyBirth:
                UpdatePosition();
                UpdateRotation();
                break;
        }
        UpdateRotation();
    }

    void LateUpdate()
    {
        switch (_updateType)
        {
            case UpdateType.Always:
                UpdatePosition();
                UpdateRotation();
                break;
            case UpdateType.OnlyBirth:

                break;
        }
    }

    void UpdatePosition()
    {
        switch (_movementType)
        {
            case MovementType.HardFollow:
                targetPosition = GetTargetPosition(_followTarget.position, _offset);
                transform.position = targetPosition;
                break;
            case MovementType.SoftFollow:
                if (Vector3.Distance(_followTarget.position, transform.position) > _needMoveDistance)
                {
                    _needMove = true;
                }
                if (_needMove)
                {
                    targetPosition = GetTargetPosition(_followTarget.position, _offset);
                    transform.position = AnimationLerp(transform.position, targetPosition, _lerpSpeed);
                    if (Vector3.Distance(_followTarget.position, transform.position) < _regardAsArriveDistance)
                    {
                        _needMove = false;
                    }
                }
                break;
            case MovementType.NotFollow:

                break;
        }
    }

    void UpdateRotation()
    {
        switch (_rotateType)
        {
            case RotateType.InvertFacing:
                transform.rotation = Quaternion.Euler(_followTarget.rotation.eulerAngles.x, _followTarget.rotation.eulerAngles.y, _followTarget.rotation.eulerAngles.z);
                break;
            case RotateType.Facing:
                transform.LookAt(_followTarget);
                break;
            case RotateType.None:
                break;
            case RotateType.CopyRotation:
                transform.rotation = _followTarget.rotation;
                break;
            case RotateType.FacingHorizentally:
                var pos = new Vector3(_followTarget.position.x,this.transform.position.y ,_followTarget.position.z);
                transform.LookAt(pos);
                break;
        }
    }

    Vector3 AnimationLerp(Vector3 position, Vector3 targetPosition, float lerpSpeed)
    {
        position += (targetPosition - position) * Time.deltaTime * lerpSpeed;
        return position;
    }

    Vector3 GetTargetPosition(Vector3 targetPosition, Vector3 offset)
    {
        Vector3 headsetForward = _followTarget.forward; // get headset forward direction
        Vector3 headsetUp = _followTarget.up; // get headset forward direction
        Vector3 headsetRight = _followTarget.right; // get headset forward direction

        var offsetByTarget = headsetForward * offset.z +
        headsetUp * offset.y +
        headsetRight * offset.x;

        return targetPosition + offsetByTarget;
    }
    public void Reset()
    {
        transform.position = _followTarget.position + _followTarget.TransformVector(_offset);
    }
}