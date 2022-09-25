using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAudioEffectList")]
    public class MofaAudioEffectList : ScriptableObject
    {
        public AudioClip FightingBackgroundMusic;

        public AudioClip ReadySound;

        public AudioClip CountdownSound;

        public AudioClip RoundOverSound;

        public AudioClip VictorySound;

        public AudioClip DefeatSound;

        public AudioClip DrawSound;
    }
}