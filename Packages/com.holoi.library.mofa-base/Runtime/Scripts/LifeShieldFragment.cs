// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public enum LifeShieldArea
    {
        Center = 0,
        Top = 1,
        Left = 2,
        Right = 3
    }

    public class LifeShieldFragment : MonoBehaviour, IDamageable
    {
        [SerializeField] private LifeShieldArea _area;

        public LifeShieldArea Area => _area;

        public void OnDamaged(ulong attackerClientId)
        {
            GetComponentInParent<LifeShield>().OnDamaged(_area, attackerClientId);
        }
    }
}