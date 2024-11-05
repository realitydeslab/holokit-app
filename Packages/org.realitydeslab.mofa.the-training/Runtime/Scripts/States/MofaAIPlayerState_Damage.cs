// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.TheTraining
{
    public class MofaAIPlayerState_Damage : MofaAIPlayerState
    {
        private bool _hasEnteredState;

        private const float BeatBackSpeed = 0.3f;

        public override void OnEnter(MofaAIPlayer player)
        {
            _hasEnteredState = false;
            player.PlayDamageAnimationClientRpc();
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
            var currentAnimatorStateInfo = player.AvatarAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.IsTag("Damage"))
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
