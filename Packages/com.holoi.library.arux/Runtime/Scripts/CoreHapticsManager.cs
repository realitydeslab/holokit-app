// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

#if APPLE_CORE_HAPTICS_ENABLED
using UnityEngine;
using Apple.CoreHaptics;
using HoloKit;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.ARUX
{
    public class CoreHapticsManager : MonoBehaviour
    {
        public CHHapticEngine Engine;

        public bool IsValid
        {
            get
            {
                return HoloKitUtils.IsRuntime && HoloKitAppLib.HoloKitApp.Instance.GlobalSettings.VibrationEnabled;
            }
        }

        private void Start()
        {
            if (IsValid)
            {
                Engine = new CHHapticEngine { IsAutoShutdownEnabled = false };
                Engine.Start();
            }
        }

        private void OnDestroy()
        {
            if (IsValid)
            {
                Engine.Stop();
            }
        }
    }
}
#endif