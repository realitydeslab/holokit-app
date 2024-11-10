// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typed.TheHair
{
    public class TypedTheHairRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        private void Start()
        {
            if (HoloKitApp.Instance.IsSpectator)
            {
                _arOcclusionManager.enabled = true;
            }
        }
    }
}