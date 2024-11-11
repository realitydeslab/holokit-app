// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitAppLib;
using BoidsSimulationOnGPU;

namespace RealityDesignLab.Typed.TheFlock
{
    public class BoidTypoRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [Header("Boid")]
        [SerializeField] private VisualEffect _boidVfx;

        [SerializeField] private GPUBoids _boidAlgorithm;

        private void Start()
        {
            _boidVfx.enabled = true;
            if (!HoloKitApp.Instance.IsSpectator)
            {
                _arOcclusionManager.enabled = true;
            }
        }

        private void Update()
        {
            _boidVfx.SetGraphicsBuffer("PositionDataBuffer", _boidAlgorithm.GetBoidPositionDataBuffer());
            _boidVfx.SetGraphicsBuffer("VelocityDataBuffer", _boidAlgorithm.GetBoidVelocityDataBuffer());
        }
    }
}