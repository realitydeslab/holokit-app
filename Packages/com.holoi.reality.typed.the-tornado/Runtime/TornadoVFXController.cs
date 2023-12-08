// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
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
