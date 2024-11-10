// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typed.TheRainbow
{
    public class RainbowController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        public float Width = 0.05f;

        void Start()
        {
        }

        void Update()
        {
            _vfx.SetFloat("Width", Width);
        }

    }
}
