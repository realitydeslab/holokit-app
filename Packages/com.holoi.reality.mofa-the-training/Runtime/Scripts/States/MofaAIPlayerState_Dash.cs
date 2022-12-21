using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAIPlayerState_Dash : MofaAIPlayerState
    {
        private enum DashDirection
        {
            Forward = 0,
            Backward = 1,
            Left = 2,
            Right = 3
        }

        private float _lastDashTime;

        private bool _hasEnteredState;

        private DashDirection _dashDirection;

        private const float DashSpeed = 3f;

        /// <summary>
        /// The probability that the avatar will dash when an attack is detected.
        /// </summary>
        private const float DashProbability = 0.3f;

        private const float DashCooldownTime = 3f;

        public override void OnEnter(MofaAIPlayer player)
        {
            _lastDashTime = Time.time;
            _hasEnteredState = false;
            _dashDirection = Random.Range(0f, 1f) > 0.5f ? DashDirection.Left : DashDirection.Right;
            player.PlayDashAnimationClientRpc((int)_dashDirection);
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
            var currentAnimatorStateInfo = player.AvatarAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.IsTag("Dash"))
            {
                _hasEnteredState = true;
                if (currentAnimatorStateInfo.normalizedTime < 0.6f)
                {
                    float sign = _dashDirection == DashDirection.Right ? 1f : -1f;
                    player.transform.position += sign * DashSpeed * Time.deltaTime * player.transform.right;
                }
            }
            else
            {
                if (_hasEnteredState)
                {
                    player.SwitchState(player.MovementState);
                }
            }
        }

        public override void OnExit(MofaAIPlayer player)
        {
            
        }

        /// <summary>
        /// Whether the avatar should dash when an attack is detected. There are two
        /// factors determining whether to dash. First, the dash has a cooldown time.
        /// If the dash is still cooling down, the avatar will not dash. Second, there
        /// is a probability to dash, we roll a dice and see if the avatar should dash.
        /// </summary>
        /// <returns></returns>
        public bool ShouldDash()
        {
            if (Time.time - _lastDashTime < DashCooldownTime)
                return false;
            return Random.Range(0f, 1f) < DashProbability;
        }
    }
}
