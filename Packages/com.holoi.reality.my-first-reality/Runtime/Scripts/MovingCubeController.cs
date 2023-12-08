// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
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
