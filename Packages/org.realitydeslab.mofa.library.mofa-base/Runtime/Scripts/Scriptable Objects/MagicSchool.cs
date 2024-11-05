// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MagicSchool")]
    public class MagicSchool : ScriptableObject
    {
        public string TokenId;

        public string Name;
    }
}