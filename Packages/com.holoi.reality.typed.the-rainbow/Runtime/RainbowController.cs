// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
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
