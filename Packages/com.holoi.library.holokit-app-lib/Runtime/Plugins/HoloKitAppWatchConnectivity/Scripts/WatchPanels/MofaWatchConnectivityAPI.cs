// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA
{
    // You must make sure this enum is identical to the one on Mofa Watch App
    public enum MofaWatchState
    {
        Normal = 0,
        Ground = 1
    }

    public static class MofaWatchConnectivityAPI
    {
        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_Initialize(Action OnReceivedStartRoundMessage,
                                                                    Action<int> OnWatchStateChanged,
                                                                    Action OnWatchTriggered,
                                                                    Action<float, float> OnReceivedHealthDataMessage);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_OnRoundStarted(int magicSchoolIndex);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_QueryWatchState();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_OnRoundEnded(int result, int kill, float hitRate);

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnReceivedStartRoundMessageDelegate()
        {
            OnReceivedStartRoundMessage?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnWatchStateChangedDelegate(int watchState)
        {
            OnWatchStateChanged?.Invoke((MofaWatchState)watchState);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnWatchTriggeredDelegate()
        {
            OnWatchTriggered?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<float, float>))]
        private static void OnReceivedHealthDataMessageDelegate(float dist, float energy)
        {
            UnityEngine.Debug.Log($"[MofaWatchConnectivityAPI] OnReceivedHealthDataMessageDelegate dist: {dist} and energy: {energy}");
            OnReceivedHealthDataMessage?.Invoke(dist, energy);
        }

        public static event Action OnReceivedStartRoundMessage;

        public static event Action<MofaWatchState> OnWatchStateChanged;

        public static event Action OnWatchTriggered;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Exercise the iPhone gameplay path after a watch wrist swing has been recognized.
        // This does not claim to test CoreMotion classification or a paired physical watch.
        public static void EmitSimulatedWatchTriggerForDeviceTest()
        {
            OnWatchTriggered?.Invoke();
        }

        // The event emitted when the watch turns to or from the Ground pose.
        public static void EmitSimulatedWatchStateForDeviceTest(MofaWatchState state)
        {
            OnWatchStateChanged?.Invoke(state);
        }

        [DllImport("__Internal")]
        private static extern void HoloKitAppWatchConnectivity_ReceiveMessageForDeviceTest(int kind, int value);

        [DllImport("__Internal")]
        private static extern int HoloKitAppWatchConnectivity_ActivationState();

        [DllImport("__Internal")]
        private static extern string MofaWatchConnectivity_LastContextError();

        /// <summary>Delivers a watch swing through the iPhone's WatchConnectivity session delegate (iOS only).</summary>
        public static void ReceiveWatchTriggerForDeviceTest()
        {
            if (HoloKit.HoloKitUtils.IsRuntime) HoloKitAppWatchConnectivity_ReceiveMessageForDeviceTest(0, 0);
        }

        /// <summary>Delivers a watch state change through the iPhone's WatchConnectivity session delegate (iOS only).</summary>
        public static void ReceiveWatchStateForDeviceTest(MofaWatchState state)
        {
            if (HoloKit.HoloKitUtils.IsRuntime) HoloKitAppWatchConnectivity_ReceiveMessageForDeviceTest(1, (int)state);
        }

        /// <summary>WCSessionActivationState (0 not activated, 1 inactive, 2 activated); -1 without a session.</summary>
        public static int SessionActivationStateForDeviceTest =>
            HoloKit.HoloKitUtils.IsRuntime ? HoloKitAppWatchConnectivity_ActivationState() : -1;

        /// <summary>Why the last round start or end did not reach the watch's context; empty when it did.</summary>
        public static string LastContextErrorForDeviceTest =>
            HoloKit.HoloKitUtils.IsRuntime ? MofaWatchConnectivity_LastContextError() : "";
#endif

        public static event Action<float, float> OnReceivedHealthDataMessage;

        public static void Initialize()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
                MofaWatchConnectivity_Initialize(OnReceivedStartRoundMessageDelegate,
                                             OnWatchStateChangedDelegate,
                                             OnWatchTriggeredDelegate,
                                             OnReceivedHealthDataMessageDelegate);
        }

        public static void OnRoundStarted(int magicSchoolIndex)
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
                MofaWatchConnectivity_OnRoundStarted(magicSchoolIndex);
        }

        public static void QueryWatchState()
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
                MofaWatchConnectivity_QueryWatchState();
        }

        public static void OnRoundEnded(int result, int kill, float hitRate)
        {
            if (HoloKit.HoloKitUtils.IsRuntime)
                MofaWatchConnectivity_OnRoundEnded(result, kill, hitRate);
        }
    }
}
