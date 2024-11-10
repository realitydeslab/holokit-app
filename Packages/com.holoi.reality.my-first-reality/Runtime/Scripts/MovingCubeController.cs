// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MyFirstReality
{
    public class MovingCubeController : MonoBehaviour
    {
        private void Update()
        {
            // Move the cube in sine wave
            transform.position = new Vector3(Mathf.Sin(Time.time), transform.position.y, transform.position.z);
        }
    }
}
