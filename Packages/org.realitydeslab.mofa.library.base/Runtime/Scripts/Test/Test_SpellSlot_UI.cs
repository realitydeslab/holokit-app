// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;

namespace RealityDesignLab.MOFA.Library.Base.Test.UI
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
