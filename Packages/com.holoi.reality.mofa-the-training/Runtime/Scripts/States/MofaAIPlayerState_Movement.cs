using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAIPlayerState_Movement : MofaAIPlayerState
    {
        /// <summary>
        /// The current movement speed of the avatar.
        /// </summary>
        private float _speed = 0.5f;

        /// <summary>
        /// The current target position the avatar is trying to reach.
        /// </summary>
        private Vector3 _targetPosition;

        private float _attackChargeTime;

        private float _attackChargeThreshold;

        /// <summary>
        /// The movement area of the avatar is a rectangle of which the avatar's
        /// spawn position is the center. This Vector2 represents the horizontal
        /// magnitude of the rectangle.
        /// </summary>
        private readonly Vector2 HorizontalMovementRange = new(-2.4f, 2.4f);

        /// <summary>
        /// This Vector2 represents the vertical magnitude of the rectangle.
        /// </summary>
        private readonly Vector2 VerticalMovementRange = new(-1.8f, 1.2f);

        /// <summary>
        /// When the distance between the avatar and the target position if less
        /// than this threshold, we think the avatar has already reached the target
        /// position.
        /// </summary>
        private const float DistanceThreshold = 0.1f;

        /// <summary>
        /// The min movement speed of the avatar.
        /// </summary>
        private const float MinSpeed = 0.2f;

        /// <summary>
        /// The max movement speed of the avatar.
        /// </summary>
        private const float MaxSpeed = 0.1f;

        /// <summary>
        /// The max absolute value for a single axis of the movement animation blend tree.
        /// </summary>
        private const float BlendTreeMagnitude = 4f;

        private const float MinAttackChargeThreshold = 2f;

        private const float MaxAttackChargeThreshold = 8f;

        public override void OnEnter(MofaAIPlayer player)
        {
            GetNewTargetPosition(player);
            UpdateAttackChargeThreshold();
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
            // Update movement
            Vector3 movingDirWithLength = _targetPosition - player.transform.position;
            if (movingDirWithLength.magnitude > DistanceThreshold)
            {
                player.transform.position += _speed * Time.deltaTime * movingDirWithLength.normalized;

                // Update velocity for animator
                Vector2 currentVelocity = new(Vector3.Dot(player.transform.right, movingDirWithLength), Vector3.Dot(player.transform.forward, movingDirWithLength));
                player.Velocity.Value = _speed / MaxSpeed * BlendTreeMagnitude * currentVelocity.normalized;
                //Debug.Log($"[AnimationControl] Velocity: {player.Velocity.Value}");
            }
            else
            {
                GetNewTargetPosition(player);
            }

            // Update attack charge
            _attackChargeTime += Time.deltaTime;
            if (_attackChargeTime > _attackChargeThreshold)
            {
                player.SwitchState(player.AttackState);
                _attackChargeTime = 0f;
                UpdateAttackChargeThreshold();
            }
        }

        public override void OnExit(MofaAIPlayer player)
        {
            player.Velocity.Value = Vector2.zero;
        }

        /// <summary>
        /// We choose the new target position based on the sector area in front
        /// of the host player.
        /// </summary>
        private void GetNewTargetPosition(MofaAIPlayer player)
        {
            float horizontalVar = Random.Range(HorizontalMovementRange.x, HorizontalMovementRange.y);
            float verticalVar = Random.Range(VerticalMovementRange.x, VerticalMovementRange.y);

            Vector3 _initialRight = Quaternion.Euler(0f, 90f, 0f) * player.InitialForward;
            _targetPosition = player.InitialPosition + horizontalVar * _initialRight + verticalVar * player.InitialForward;
        }

        private void UpdateAttackChargeThreshold()
        {
            _attackChargeThreshold = Random.Range(MinAttackChargeThreshold, MaxAttackChargeThreshold);
        }
    }
}
