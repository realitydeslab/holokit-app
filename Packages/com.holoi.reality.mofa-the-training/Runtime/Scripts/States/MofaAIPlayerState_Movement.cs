using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAIPlayerState_Movement : MofaAIPlayerState
    {
        private float _speed = 0.5f;

        private Vector3 _targetPosition;

        private readonly Vector2 HorizontalMovementRange = new(-2.4f, 2.4f);

        private readonly Vector2 VerticalMovementRange = new(-1.8f, 1.2f);

        private const float DistanceThreshold = 0.1f;

        private const float MinSpeed = 0.3f;

        private const float MaxSpeed = 0.8f;

        private const float BlendTreeMagnitude = 4f;

        public override void OnEnter(MofaAIPlayer player)
        {
            GetNewTargetPosition(player);
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
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
        }

        public override void OnExit(MofaAIPlayer player)
        {
            
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
    }
}
