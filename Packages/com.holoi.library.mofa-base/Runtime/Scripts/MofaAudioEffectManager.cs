using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaAudioEffectManager : MonoBehaviour
    {
        [SerializeField] private MofaAudioEffectList _mofaAudioEffectList;

        [SerializeField] private AudioSource _bgmAudioSource;

        [SerializeField] private AudioSource _effectAudioSource;

        private void Awake()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
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
                case MofaPhase.RoundData:
                    break;
            }
        }

        private void StartPlayingFightingBGM()
        {
            if (_mofaAudioEffectList.FightingBackgroundMusic != null)
            {
                _bgmAudioSource.clip = _mofaAudioEffectList.FightingBackgroundMusic;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.Play();
            }
        }

        private void StopPlayingFightingBGM()
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

        private void PlayVictorySound()
        {
            if (_mofaAudioEffectList.VictorySound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.VictorySound;
                _effectAudioSource.Play();
            }
        }

        private void PlayDefeatSound()
        {
            if (_mofaAudioEffectList.DefeatSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.DefeatSound;
                _effectAudioSource.Play();
            }
        }

        private void PlayDrawSound()
        {
            if (_mofaAudioEffectList.DrawSound != null)
            {
                _effectAudioSource.clip = _mofaAudioEffectList.DrawSound;
                _effectAudioSource.Play();
            }
        }

        private void OnRoundResult()
        {
            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            if (mofaRealityManager.IsLocalPlayerSpectator())
            {
                return;
            }
            var localPlayer = mofaRealityManager.GetLocalPlayer();
            var roundResult = mofaRealityManager.GetRoundResult();
            switch (roundResult)
            {
                case MofaRoundResult.BlueTeamWins:
                    if (localPlayer.Team.Value == MofaTeam.Blue)
                    {
                        PlayVictorySound();
                    }
                    else
                    {
                        PlayDefeatSound();
                    }
                    break;
                case MofaRoundResult.RedTeamWins:
                    if (localPlayer.Team.Value == MofaTeam.Blue)
                    {
                        PlayDefeatSound();
                    }
                    else
                    {
                        PlayVictorySound();
                    }
                    break;
                case MofaRoundResult.Draw:
                    PlayDrawSound();
                    break;
            }
        }
    }
}