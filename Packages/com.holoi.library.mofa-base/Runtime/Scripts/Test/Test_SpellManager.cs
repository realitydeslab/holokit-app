using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_SpellManager : MonoBehaviour
    {
        public SpellList SpellList;

        private Spell _currentSpell;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        public void OnSpellSelected(int spellId)
        {
            _currentSpell = SpellList.GetSpell(spellId);
        }
    }
}
