// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFA.TheHunting
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
