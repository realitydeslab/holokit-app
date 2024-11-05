// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using org.realitydeslab.MOFABase;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class MofaHuntingAudioEffectManager : MofaAudioEffectManager
    {
        [SerializeField] private AudioClip _huntingBGM;

        [SerializeField] private bool _playBGM = true;

        protected override void StartPlayingFightingBGM()
        {
            if (_huntingBGM != null && _playBGM)
            {
                _bgmAudioSource.clip = _huntingBGM;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.Play();
            }
        }

        protected override void OnRoundResult()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                PlayDefeatSound();
            }
            else if (HoloKitApp.Instance.IsPlayer)
            {
                PlayVictorySound();
            }
        }
    }
}
