// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

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