// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public enum SpellType
    {
        Basic = 0,
        Secondary = 1
    }

    public class Spell : MonoBehaviour
    {
        public int Id;

        public string Name;

        public MagicSchool MagicSchool;

        public SpellType SpellType;

        public Vector3 SpawnOffset;

        public bool PerpendicularToGround;

        public float ChargeTime;

        public int MaxChargeCount;

        public int MaxUseCount;
    }
}