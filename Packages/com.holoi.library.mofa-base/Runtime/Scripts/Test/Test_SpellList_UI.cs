// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase.Test.UI
{
    public class Test_SpellList_UI : MonoBehaviour
    {
        [SerializeField] private Test_SpellManager _spellManager;

        [SerializeField] private Test_SpellSlot_UI _spellSlotPrefab;

        [SerializeField] private RectTransform _listRoot;

        private readonly List<Test_SpellSlot_UI> _spellSlotList = new();

        private void Start()
        {
            InitSpellList();
        }

        private void InitSpellList()
        {
            foreach (var spell in _spellManager.SpellList.List)
            {
                var spellSlotInstance = Instantiate(_spellSlotPrefab, _listRoot);
                spellSlotInstance.transform.localPosition = Vector3.zero;
                spellSlotInstance.transform.localRotation = Quaternion.identity;
                spellSlotInstance.transform.localScale = Vector3.one;

                spellSlotInstance.SpellName.text = spell.Name;
                spellSlotInstance.SpellId = spell.Id;
                spellSlotInstance.SpellListUI = this;

                _spellSlotList.Add(spellSlotInstance);
            }

            OnSpellSelected(1);
        }

        public void OnSpellSelected(int spellId)
        {
            foreach (var spellSlot in _spellSlotList)
            {
                if (spellSlot.SpellId == spellId)
                {
                    spellSlot.Tick.SetActive(true);
                }
                else
                {
                    spellSlot.Tick.SetActive(false);
                }
            }

            _spellManager.OnSpellSelected(spellId);
        }
    }
}
