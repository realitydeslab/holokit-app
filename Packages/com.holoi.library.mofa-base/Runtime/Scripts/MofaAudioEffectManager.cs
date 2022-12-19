using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
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

        private void OnReceivedRoundResult(MofaGeneralRoundResult roundResult)
        {
            //if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
            //{
            //    // TODO: Play blue team wins or red team wins on spectator
            //    return;
            //}

            //if (roundResult == MofaRoundResult.Draw)
            //{
            //    PlayDrawSound();
            //    return;
            //}

            //MofaTeam team = ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).GetPlayer().Team.Value;
            //if (team == MofaTeam.Blue)
            //{
            //    if (roundResult == MofaRoundResult.BlueTeamWins)
            //    {
            //        PlayVictorySound();
            //    }
            //    else
            //    {
            //        PlayDefeatSound();
            //    }
            //}
            //else if (team == MofaTeam.Red)
            //{
            //    if(roundResult == MofaRoundResult.RedTeamWins)
            //    {
            //        PlayVictorySound();
            //    }
            //    else
            //    {
            //        PlayDefeatSound();
            //    }
            //}
        }
    }
}