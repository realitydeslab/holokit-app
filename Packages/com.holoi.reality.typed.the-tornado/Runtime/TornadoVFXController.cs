// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typed.TheTornado
{
    public class TornadoVFXController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        [SerializeField] AngularVelocityCalculator _AVC;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            _vfx.SetFloat("Angular Velocity", _AVC.AngularVelocityY/90f);
        }
    }
}
