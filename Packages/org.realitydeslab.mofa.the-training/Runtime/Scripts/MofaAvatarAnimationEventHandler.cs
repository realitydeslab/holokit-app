// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.TheTraining
{
    public class MofaAvatarAnimationEventHandler : MonoBehaviour
    {
        private MofaAIPlayer _mofaAIPlayer;

        private void Start()
        {
            _mofaAIPlayer = GetComponentInParent<MofaAIPlayer>();
        }

        public void FootL()
        {
            _mofaAIPlayer.AnimationEventReceiver_FootL();
        }

        public void FootR()
        {
            _mofaAIPlayer.AnimationEventReceiver_FootR();
        }

        public void Land()
        {
            _mofaAIPlayer.AnimationEventReceiver_Land();
        }

        public void Hit()
        {
            _mofaAIPlayer.AnimationEventReceiver_Hit();
        }

        public void Shoot()
        {
            _mofaAIPlayer.AnimationEventReceiver_Shoot();
        }
    }
}
