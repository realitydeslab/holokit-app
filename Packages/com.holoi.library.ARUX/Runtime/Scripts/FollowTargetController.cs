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

        [Header("Position")]
        [SerializeField] private MovementType _movementType;

        [SerializeField] private bool _syncX = true;

        [SerializeField] private bool _syncY = true;

        [SerializeField] private bool _syncZ = true;

        [SerializeField] private bool _heightIdenticalToTarget;

        [SerializeField] private Vector3 _offset;

        [SerializeField] private float _lerpSpeed;

        [SerializeField] private float _lerpThreshold;

        [Header("Rotation")]
        [SerializeField] private RotationType _rotationType;

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

            Vector3 targetPosition = _targetTransform.position + _targetTransform.TransformVector(_offset);
            if (_heightIdenticalToTarget)
            {
                targetPosition = new(targetPosition.x, _targetTransform.position.y, targetPosition.z);
            }
            switch (_movementType)
            {
                case MovementType.Instant:
                    //transform.position = realTargetPosition;
                    transform.position = new Vector3(_syncX ? targetPosition.x : transform.position.x,
                                                     _syncY ? targetPosition.y : transform.position.y,
                                                     _syncZ ? targetPosition.z : transform.position.z);
                    break;
                case MovementType.Lerp:
                    float distance = Vector3.Distance(transform.position, targetPosition);
                    if (distance > _lerpThreshold)
                    {
                        Vector3 newPosition = transform.position + _lerpSpeed * Time.deltaTime * (targetPosition - transform.position).normalized;
                        transform.position = new Vector3(_syncX ? newPosition.x : transform.position.x,
                                                         _syncY ? newPosition.y : transform.position.y,
                                                         _syncZ ? newPosition.z : transform.position.z);
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