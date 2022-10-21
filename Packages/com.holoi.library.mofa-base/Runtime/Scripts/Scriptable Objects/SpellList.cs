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