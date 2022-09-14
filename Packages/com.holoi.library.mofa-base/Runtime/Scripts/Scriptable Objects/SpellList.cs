using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SpellList")]
    public class SpellList : ScriptableObject
    {
        public List<Spell> List;
    }
}