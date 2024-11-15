// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.Base
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