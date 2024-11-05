// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SpellList")]
    public class SpellList : ScriptableObject
    {
        public List<Spell> List;

        // This is a safer way to get spells
        public Spell GetSpell(int id)
        {
            foreach (var spell in List)
            {
                if (spell.Id == id)
                {
                    return spell;
                }
            }
            return null;
        }
    }
}