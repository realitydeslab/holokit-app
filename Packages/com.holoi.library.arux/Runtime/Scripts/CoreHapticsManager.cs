// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Apple.CoreHaptics;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class CoreHapticsManager : MonoBehaviour
    {
        public CHHapticEngine Engine;

        public bool IsValid
        {
            get
            {
                return HoloKitUtils.IsRuntime && HoloKitApp.HoloKitApp.Instance.GlobalSettings.VibrationEnabled;
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
