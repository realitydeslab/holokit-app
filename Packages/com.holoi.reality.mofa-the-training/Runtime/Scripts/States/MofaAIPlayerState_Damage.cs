using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAIPlayerState_Damage : MofaAIPlayerState
    {
        private bool _hasEnteredState;

        private const float BeatBackSpeed = 0.3f;

        public override void OnEnter(MofaAIPlayer player)
        {
            _hasEnteredState = false;
            player.PlayDamageClientRpc();
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
            var currentAnimatorStateInfo = player.AvatarAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.IsName("LightHit"))
            {
                _hasEnteredState = true;
                if (currentAnimatorStateInfo.normalizedTime < 0.6f)
                    player.transform.position -= BeatBackSpeed * Time.deltaTime * player.transform.forward;
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
    }
}
