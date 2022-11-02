using UnityEngine;

namespace Holoi.Library.ARUX
{
    public enum MovementType
    {
        None = 0,
        Instant = 1,
        Lerp = 2
    }

    public enum RotationType
    {
        None = 0,
        IdenticalToTarget = 1,
        FacingTarget = 2,
        PerpendicularToTheGround = 3
    }

    public class FollowTargetController : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;

        [SerializeField] private Vector3 _offset;

        [SerializeField] private float _lerpSpeed;

        [SerializeField] private float _lerpThreshold;

        [SerializeField] private MovementType _movementType;

        [SerializeField] private RotationType _rotationType;

        [SerializeField] private bool _heightIdenticalToTarget;

        public MovementType MovementType
        {
            get => _movementType;
            set
            {
                _movementType = value;
            }
        }

        public RotationType RotationType
        {
            get => _rotationType;
            set
            {
                _rotationType = value;
            }
        }

        private void Start()
        {
            if (_targetTransform == null)
            {
                return;
            }

            transform.SetPositionAndRotation(_targetTransform.position + _targetTransform.TransformVector(_offset),
                                             _targetTransform.rotation);
        }

        private void LateUpdate()
        {
            if (_targetTransform == null)
            {
                return;
            }

            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            if (_movementType == MovementType.None)
            {
                return;
            }

            Vector3 realTargetPosition = _targetTransform.position + _targetTransform.TransformVector(_offset);
            if (_heightIdenticalToTarget)
            {
                realTargetPosition = new(realTargetPosition.x, _targetTransform.position.y, realTargetPosition.z);
            }
            switch (_movementType)
            {
                case MovementType.Instant:
                    transform.position = realTargetPosition;
                    break;
                case MovementType.Lerp:
                    float distance = Vector3.Distance(transform.position, realTargetPosition);
                    if (distance > _lerpThreshold)
                    {
                        transform.position += _lerpSpeed * Time.deltaTime * (realTargetPosition - transform.position).normalized;
                    }
                    break;
            }
        }

        private void UpdateRotation()
        {
            switch (_rotationType)
            {
                case RotationType.None:
                    break;
                case RotationType.IdenticalToTarget:
                    transform.rotation = _targetTransform.rotation;
                    break;
                case RotationType.FacingTarget:
                    transform.LookAt(_targetTransform);
                    break;
                case RotationType.PerpendicularToTheGround:
                    transform.rotation = Quaternion.Euler(0f, _targetTransform.rotation.eulerAngles.y, 0f);
                    break;
            }
        }
    }
}