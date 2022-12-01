using System;
using System.Runtime.InteropServices;
using Holoi.Library.HoloKitApp.WatchConnectivity;

namespace Holoi.Library.MOFABase.WatchConnectivity
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
        private static extern void MofaWatchConnectivity_Initialize(Action<bool> OnSessionReachabilityChanged,
                                                                    Action OnReceivedStartRoundMessage,
                                                                    Action<int> OnWatchStateChanged,
                                                                    Action OnWatchTriggered,
                                                                    Action<float, float> OnReceivedHealthDataMessage);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_TakeControlWCSession();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_SyncRoundStartToWatch(int magicSchool);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_SyncRoundResultToWatch(int roundResultIndex, int kill, float hitRate);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_QueryWatchState();

        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void OnSessionReachabilityChangedDelegate(bool isReachable)
        {
            HoloKitAppWatchConnectivityAPI.OnSessionReachabilityChangedDelegate(isReachable);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnReceivedStartRoundMessageDelegate()
        {
            OnReceivedStartRoundMessage?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnWatchStateChangedDelegate(int watchStateIndex)
        {
            OnWatchStateChanged?.Invoke((MofaWatchState)watchStateIndex);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnWatchTriggeredDelegate()
        {
            OnWatchTriggered?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<float, float>))]
        private static void OnReceivedHealthDataMessageDelegate(float distance, float calories)
        {
            OnReceivedHealthDataMessage?.Invoke(distance, calories);
            UnityEngine.Debug.Log($"[MofaWatchConnectivity] Health data distance: {distance} and calories: {calories}");
        }

        public static event Action OnReceivedStartRoundMessage;

        public static event Action<MofaWatchState> OnWatchStateChanged;

        public static event Action OnWatchTriggered;

        public static event Action<float, float> OnReceivedHealthDataMessage;

        public static void Initialize()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_Initialize(OnSessionReachabilityChangedDelegate,
                                             OnReceivedStartRoundMessageDelegate,
                                             OnWatchStateChangedDelegate,
                                             OnWatchTriggeredDelegate,
                                             OnReceivedHealthDataMessageDelegate);
        }

        public static void TakeControlWatchConnectivitySession()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_TakeControlWCSession();
        }

        public static void SyncRoundStartToWatch(int magicSchool)
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_SyncRoundStartToWatch(magicSchool);
        }

        public static void SyncRoundResultToWatch(MofaIndividualRoundResult roundResult, int kill, float hitRate)
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_SyncRoundResultToWatch((int)roundResult, kill, hitRate);
        }

        public static void QueryWatchState()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_QueryWatchState();
        }
    }
}
