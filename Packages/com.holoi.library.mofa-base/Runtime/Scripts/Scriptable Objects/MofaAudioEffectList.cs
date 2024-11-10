// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
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