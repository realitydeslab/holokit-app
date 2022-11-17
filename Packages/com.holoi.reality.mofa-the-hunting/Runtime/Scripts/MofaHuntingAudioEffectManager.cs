using UnityEngine;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class MofaHuntingAudioEffectManager : MofaAudioEffectManager
    {
        [SerializeField] private AudioClip _huntingBGM;

        protected override void StartPlayingFightingBGM()
        {
            if (_huntingBGM != null)
            {
                _bgmAudioSource.clip = _huntingBGM;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.Play();
            }
        }
    }
}
