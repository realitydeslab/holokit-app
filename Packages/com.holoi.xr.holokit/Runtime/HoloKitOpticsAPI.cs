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

        public static Vector3 GetPhoneModelCameraOffset(HoloKitType holokitType)
        {
            IntPtr offsetPtr = HoloKitSDK_GetPhoneModelCameraOffsetPtr((int)holokitType);
            float[] offset = new float[3];
            Marshal.Copy(offsetPtr, offset, 0, 3);
            HoloKitSDK_ReleasePhoneModelCameraOffsetPtr(offsetPtr);
            return new Vector3(offset[0], offset[1], offset[2]);
        }
    }
}
