using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HoloKit
{
    public static class HoloKitHandTrackingControllerAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SetHandTrackingActive(bool active);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_RegisterHandTrackingControllerDelegates(Action<IntPtr> OnHandPoseUpdated);

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr>))]
        private static void OnHandPoseUpdatedDelegate(IntPtr ptr)
        {
            float[] data = new float[63];
            Marshal.Copy(ptr, data, 0, 63);
            OnHandPoseUpdated?.Invoke(data);
        }

        public static event Action<float[]> OnHandPoseUpdated;

        public static void SetHandTrackingActive(bool active)
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitSDK_SetHandTrackingActive(active);
            }
        }

        public static void RegisterHandTrackingControllerDelegates()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitSDK_RegisterHandTrackingControllerDelegates(OnHandPoseUpdatedDelegate);
            }
        }
    }
}