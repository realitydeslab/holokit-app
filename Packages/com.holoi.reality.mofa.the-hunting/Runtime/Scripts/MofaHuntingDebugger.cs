// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
