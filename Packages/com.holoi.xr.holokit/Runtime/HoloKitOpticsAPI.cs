// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HoloKit
{
    public static class HoloKitOpticsAPI
    {
        [DllImport("__Internal")]
        private static extern IntPtr HoloKitSDK_GetPhoneModelCameraOffsetPtr(int holokitType);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_ReleasePhoneModelCameraOffsetPtr(IntPtr ptr);

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetPhoneModelScreenDpi();

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetHoloKitModelPhoneFrameWidthInPixel();

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetHoloKitModelPhoneFrameHeightInPixel();

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetHoloKitModelHorizontalAlignmentMarkerOffsetInPixel(int holokitType);

        [DllImport("__Internal")]
        private static extern bool HoloKitSDK_IsCurrentDeviceSupportedByHoloKit();

        [DllImport("__Internal")]
        private static extern bool HoloKitSDK_IsCurrentDeviceIpad();

        [DllImport("__Internal")]
        private static extern bool HoloKitSDK_IsCurrentDeviceEquippedWithLiDAR();

        public static Vector3 GetPhoneModelCameraOffset(HoloKitType holokitType)
        {
            IntPtr offsetPtr = HoloKitSDK_GetPhoneModelCameraOffsetPtr((int)holokitType);
            float[] offset = new float[3];
            Marshal.Copy(offsetPtr, offset, 0, 3);
            HoloKitSDK_ReleasePhoneModelCameraOffsetPtr(offsetPtr);
            return new Vector3(offset[0], offset[1], offset[2]);
        }

        public static float GetScreenDpi()
        {
            if (HoloKitUtils.IsRuntime)
            {
                return HoloKitSDK_GetPhoneModelScreenDpi();
            }
            else
            {
                return Screen.dpi;
            }
        }

        // Vector.x is width and Vector.y is height
        public static Vector2 GetHoloKitModelPhoneFrameSizeInPixel()
        {
            float width = HoloKitSDK_GetHoloKitModelPhoneFrameWidthInPixel();
            float height = HoloKitSDK_GetHoloKitModelPhoneFrameHeightInPixel();
            return new Vector2(width, height);
        }

        public static float GetHoloKitModelHorizontalAlignmentMarkerOffsetInPixel(HoloKitType holokitType)
        {
            return HoloKitSDK_GetHoloKitModelHorizontalAlignmentMarkerOffsetInPixel((int)holokitType);
        }

        public static bool IsCurrentDeviceSupportedByHoloKit()
        {
            if (HoloKitUtils.IsEditor) { return true; }
            return HoloKitSDK_IsCurrentDeviceSupportedByHoloKit();
        }

        public static bool IsCurrentDeviceIpad()
        {
            if (HoloKitUtils.IsEditor) { return false; }
            return HoloKitSDK_IsCurrentDeviceIpad();
        }

        public static bool IsCurrentDeviceEquippedWithLiDAR()
        {
            if (HoloKitUtils.IsEditor) { return true; }
            return HoloKitSDK_IsCurrentDeviceEquippedWithLiDAR();
        }
    }
}
