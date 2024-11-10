// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MOFA.TheHunting
{
    public class MofaHuntingDebugger : MonoBehaviour
    {
        public void OnAxisChanged(Vector2 value)
        {
            Debug.Log($"OnAxisChanged: {value}");
        }
    }
}
