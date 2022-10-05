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
        private static extern float HoloKitSDK_GetHoloKitModelPhoneFrameWidth(int holokitType);

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetHoloKitModelPhoneFrameHeight(int holokitType);

        [DllImport("__Internal")]
        private static extern float HoloKitSDK_GetHoloKitModelHorizontalAlignmentMarkerOffset(int holokitType);

        public static Vector3 GetPhoneModelCameraOffset(HoloKitType holokitType)
        {
            IntPtr offsetPtr = HoloKitSDK_GetPhoneModelCameraOffsetPtr((int)holokitType);
            float[] offset = new float[3];
            Marshal.Copy(offsetPtr, offset, 0, 3);
            HoloKitSDK_ReleasePhoneModelCameraOffsetPtr(offsetPtr);
            return new Vector3(offset[0], offset[1], offset[2]);
        }

        // Vector.x is width and Vector.y is height
        public static Vector2 GetHoloKitModelPhoneFrameSize(HoloKitType holokitType)
        {
            float width = HoloKitSDK_GetHoloKitModelPhoneFrameWidth((int)holokitType);
            float height = HoloKitSDK_GetHoloKitModelPhoneFrameHeight((int)holokitType);
            return new Vector2(width, height);
        }

        public static float GetHoloKitModelHorizontalAlignmentMarkerOffset(HoloKitType holokitType)
        {
            return HoloKitSDK_GetHoloKitModelHorizontalAlignmentMarkerOffset((int)holokitType);
        }
    }
}
