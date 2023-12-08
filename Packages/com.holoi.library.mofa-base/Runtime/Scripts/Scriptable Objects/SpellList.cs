// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
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