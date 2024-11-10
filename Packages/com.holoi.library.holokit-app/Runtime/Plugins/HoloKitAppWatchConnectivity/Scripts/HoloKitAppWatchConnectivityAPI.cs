// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using HoloKit;

namespace Holoi.Library.HoloKitApp.WatchConnectivity
{
    // You muse make sure this enum is identical to the enum on Watch App.
    public enum HoloKitWatchPanel
    {
        None = 0,
        MOFA = 1
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
        private static extern void HoloKitAppWatchConnectivity_UpdateWatchPanel(int watchPanel);

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        public static void OnSessionReachabilityChangedDelegate(bool isReachable)
        {
            OnSessionReachabilityChanged?.Invoke(isReachable);
        }

        public static event Action<bool> OnSessionReachabilityChanged;

        public static void ActivateWatchConnectivitySession()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_ActivateWCSession(OnSessionReachabilityChangedDelegate);
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

        public static void UpdateWatchPanel(HoloKitWatchPanel watchPanel)
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitAppWatchConnectivity_UpdateWatchPanel((int)watchPanel);
            }
        }
    }
}
