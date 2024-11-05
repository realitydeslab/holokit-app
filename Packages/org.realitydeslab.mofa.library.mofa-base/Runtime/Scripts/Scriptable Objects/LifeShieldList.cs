// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LifeShieldList")]
    public class LifeShieldList : ScriptableObject
    {
        public List<LifeShield> List;

        public LifeShield DefaultLifeShield;

        public LifeShield GetLifeShield(int index)
        {
            foreach (var lifeShield in List)
            {
                if (int.Parse(lifeShield.MagicSchool.TokenId) == index)
                {
                    return lifeShield;
                }
            }
            return DefaultLifeShield;
        }
    }
}
