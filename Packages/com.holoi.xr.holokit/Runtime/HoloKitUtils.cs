// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace HoloKit
{
    public static class HoloKitUtils
    {
        public static bool IsEditor => Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool IsRuntime => Application.platform == RuntimePlatform.IPhonePlayer;

        public const float MeterToInchRatio = 39.3701f;

        public static float PixelToMeter(float pixel)
        {
            return pixel / HoloKitOpticsAPI.GetScreenDpi() / MeterToInchRatio;
        }

        public static float MeterToPixel(float meter)
        {
            return meter * MeterToInchRatio * HoloKitOpticsAPI.GetScreenDpi();
        }
    }
}
