using UnityEngine;
using TMPro;

namespace Holoi.Library.MOFABase.Test.UI
{
    public class Test_SpellSlot_UI : MonoBehaviour
    {
        [SerializeField] private GameObject _tick;

        [SerializeField] private TMP_Text _spellName;

        public GameObject Tick => _tick;

        public TMP_Text SpellName => _spellName;

        public int SpellId { get; set; }

        public Test_SpellList_UI SpellListUI { get; set; }

        public void OnSelected()
        {
            SpellListUI.OnSpellSelected(SpellId);
        }
    }
}
