using System;
using System.Runtime.InteropServices;
using HoloKit;

namespace Holoi.Library.HoloKitApp.WatchConnectivity
{
    // You muse make sure this enum is identical to the enum on Watch App.
    public enum WatchReality
    {
        Nothing = 0,
        MOFATheTraining = 1
    }

    public static class HoloKitAppWatchConnectivityAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitAppWatchConnectivity_ActivateWCSession(Action<bool> OnSessionReachabilityChanged);

        [DllImport("__Internal")]
        private static extern bool HoloKitAppWatchConnectivity_DeviceHasPairedAppleWatch();

        [DllImport("__Internal")]
        private static extern bool HoloKitAppWatchConnectivity_IsWatchAppInstalled();

        [DllImport("__Internal")]
        private static extern bool HoloKitAppWatchConnectivity_IsWatchReachable();

        [DllImport("__Internal")]
        private static extern void HoloKitAppWatchConnectivity_TakeControlWCSession();

        [DllImport("__Internal")]
        private static extern void HoloKitAppWatchConnectivity_UpdateCurrentReality(int realityIndex);

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void OnSessionReachabilityChangedFunc(bool isReachable)
        {
            OnSessionReachabilityChanged?.Invoke(isReachable);
        }

        public static event Action<bool> OnSessionReachabilityChanged;

        public static void ActivateWatchConnectivitySession()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_ActivateWCSession(OnSessionReachabilityChangedFunc);
            }
        }

        public static bool DeviceHasPairedAppleWatch()
        {
            if (HoloKitUtils.IsRuntime)
            {
                return HoloKitAppWatchConnectivity_DeviceHasPairedAppleWatch();
            }
            else
            {
                return false;
            }
        }

        public static bool IsWatchAppInstalled()
        {
            if (HoloKitUtils.IsRuntime)
            {
                return HoloKitAppWatchConnectivity_IsWatchAppInstalled();
            }
            else
            {
                return false;
            }
        }

        public static bool IsWatchReachable()
        {
            if (HoloKitUtils.IsRuntime)
            {
                return HoloKitAppWatchConnectivity_IsWatchReachable();
            }
            else
            {
                return false;
            }
        }

        public static void TakeControlWatchConnectivitySession()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_TakeControlWCSession();
            }
        }

        public static void UpdateCurrentReality(WatchReality watchReality)
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_UpdateCurrentReality((int)watchReality);
            }
        }
    }
}
