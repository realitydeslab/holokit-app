// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.MOFABase
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