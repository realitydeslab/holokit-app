// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typed.TheMimic
{
    public class TypedTheMimicRealityManager : RealityManager
    {
        void Start()
        {
            HoloKit.HoloKitHandTracker.Instance.IsActive = true;
        }

        void Update()
        {

        }
    }
}