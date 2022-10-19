using System.Runtime.InteropServices;

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
        private static extern void HoloKitAppWatchConnectivity_ActivateWCSession();

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

        public static void ActivateWatchConnectivitySession()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_ActivateWCSession();
            }
        }

        public static bool DeviceHasPairedAppleWatch()
        {
            return HoloKitAppWatchConnectivity_DeviceHasPairedAppleWatch();
        }

        public static bool IsWatchAppInstalled()
        {
            return HoloKitAppWatchConnectivity_IsWatchAppInstalled();
        }

        public static bool IsWatchReachable()
        {
            return HoloKitAppWatchConnectivity_IsWatchReachable();
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
