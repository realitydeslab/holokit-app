// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Reality.MOFA.TheTraining
{
    public class MofaAIPlayerState_Attack : MofaAIPlayerState
    {
        private enum AttackType
        {
            // For basic spell
            SingleBasic = 0,
            TripleBasic = 1,
            // For secondary spell
            SpecialAttack1 = 2,
            SpecialAttack2 = 3,
            MoveAttack = 4
        }

        private bool _hasEnteredState;

        private float _attackMovingSpeed;

        /// <summary>
        /// 0.8 for basic spell and 0.2 for secondary spell.
        /// </summary>
        private const float BasicSpellProbability = 0.68f;

        public override void OnEnter(MofaAIPlayer player)
        {
            _hasEnteredState = true;
            bool isBasicSpell = Random.Range(0f, 1f) < BasicSpellProbability;
            AttackType attackType;
            if (isBasicSpell)
            {
                player.NextSpellType = Library.MOFABase.SpellType.Basic;
                float prob = Random.Range(0f, 1f);
                if (prob < 0.8f)
                {
                    attackType = AttackType.SingleBasic;
                    _attackMovingSpeed = 0f;
                }
                else
                {
                    attackType = AttackType.TripleBasic;
                    _attackMovingSpeed = 0.1f;
                }
            }
            else
            {
                player.NextSpellType = Library.MOFABase.SpellType.Secondary;
                float prob = Random.Range(0f, 1f);
                if (prob < 0.3f)
                {
                    attackType = AttackType.SpecialAttack1;
                    _attackMovingSpeed = 0f;
                }
                else if (prob < 0.6)
                {
                    attackType = AttackType.SpecialAttack2;
                    _attackMovingSpeed = 0f;
                }
                else
                {
                    attackType = AttackType.MoveAttack;
                    _attackMovingSpeed = 0.16f;
                }
            }
            player.PlayAttackAnimationClientRpc((int)attackType);
        }

        public override void OnUpdate(MofaAIPlayer player)
        {
            var currentAnimatorStateInfo = player.AvatarAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.IsTag("Attack"))
            {
                _hasEnteredState = true;
                if (currentAnimatorStateInfo.normalizedTime < 0.6f)
                {
                    player.transform.position += _attackMovingSpeed * Time.deltaTime * player.transform.forward;
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
    }
}
