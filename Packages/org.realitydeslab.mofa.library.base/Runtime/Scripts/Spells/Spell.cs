// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.Base
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