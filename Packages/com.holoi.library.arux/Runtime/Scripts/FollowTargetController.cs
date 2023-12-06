using UnityEngine;

namespace Holoi.Library.ARUX
{
    public enum MovementType
    {
        None = 0,
        Instant = 1,
        Smooth = 2
    }

    public enum RotationType
    {
        None = 0,
        IdenticalToTarget = 1,
        FacingTarget = 2,
        PerpendicularToTheGround = 3,
        IdentitalToTargetWithGravity = 4
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

        [SerializeField] private float _smoothTime;

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

        private Vector3 _velocity = Vector3.zero;

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
            //Vector3 targetPosition = _targetTransform.position + _offset;
            if (_heightIdenticalToTarget)
            {
                targetPosition = new(targetPosition.x, _targetTransform.position.y, targetPosition.z);
            }
            switch (_movementType)
            {
                case MovementType.Instant:
                    transform.position = new Vector3(_syncX ? targetPosition.x : transform.position.x,
                                                     _syncY ? targetPosition.y : transform.position.y,
                                                     _syncZ ? targetPosition.z : transform.position.z);
                    break;
                case MovementType.Smooth:
                    Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
                    transform.position = new Vector3(_syncX ? newPosition.x : transform.position.x,
                                                     _syncY ? newPosition.y : transform.position.y,
                                                     _syncZ ? newPosition.z : transform.position.z);
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
                case RotationType.IdentitalToTargetWithGravity:
                    Vector3 targetRotationEuler = _targetTransform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(targetRotationEuler.x, targetRotationEuler.y, 0f);
                    break;
            }
        }
    }
}