// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class MofaHuntingDebugger : MonoBehaviour
    {
        public void OnAxisChanged(Vector2 value)
        {
            Debug.Log($"OnAxisChanged: {value}");
        }
    }
}
