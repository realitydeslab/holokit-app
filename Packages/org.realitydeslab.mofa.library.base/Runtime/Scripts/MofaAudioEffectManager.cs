// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    public class MofaAudioEffectManager : MonoBehaviour
    {
        [SerializeField] private MofaAudioEffectList _mofaAudioEffectList;

        [SerializeField] protected AudioSource _bgmAudioSource;

        [SerializeField] private AudioSource _effectAudioSource;

        private void Awake()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged += OnPhaseChanged;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    PlayCountdownSound();
                    break;
                case MofaPhase.Fighting:
                    StartPlayingFightingBGM();
                    break;
                case MofaPhase.RoundOver:
                    StopPlayingFightingBGM();
                    PlayRoundOverSound();
                    break;
                case MofaPhase.RoundResult:
                    OnRoundResult();
                    break;
            }
        }

        protected virtual void StartPlayingFightingBGM()
        {
            if (_mofaAudioEffectList.FightingBackgroundMusic != null)
            {
                _bgmAudioSource.clip = _mofaAudioEffectList.FightingBackgroundMusic;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.Play();
            }
        }

        protected virtual void StopPlayingFightingBGM()
        {
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }
        }

        private void PlayCountdownSound()
        {
            if (_mofaAudioEffectList.CountdownSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.CountdownSound;
                _effectAudioSource.Play();
            }
        }

        private void PlayRoundOverSound()
        {
            if (_mofaAudioEffectList.RoundOverSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.RoundOverSound;
                _effectAudioSource.Play();
            }
        }

        protected void PlayVictorySound()
        {
            if (_mofaAudioEffectList.VictorySound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.VictorySound;
                _effectAudioSource.Play();
            }
        }

        protected void PlayDefeatSound()
        {
            if (_mofaAudioEffectList.DefeatSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.DefeatSound;
                _effectAudioSource.Play();
            }
        }

        protected void PlayDrawSound()
        {
            if (_mofaAudioEffectList.DrawSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.DrawSound;
                _effectAudioSource.Play();
            }
        }

        protected virtual void OnRoundResult()
        {
            if (HoloKitApp.Instance.IsSpectator)
                return;

            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var localMofaPlayer = mofaBaseRealityManager.LocalMofaPlayer;
            var personalRoundResult = mofaBaseRealityManager.GetPersonalRoundResult(localMofaPlayer);
            switch (personalRoundResult)
            {
                case MofaPersonalRoundResult.Victory:
                    PlayVictorySound();
                    break;
                case MofaPersonalRoundResult.Defeat:
                    PlayDefeatSound();
                    break;
                default:
                    PlayDrawSound();
                    break;
            }
        }
    }
}