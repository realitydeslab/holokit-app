using System;
using System.Runtime.InteropServices;

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
        private static extern void MofaWatchConnectivity_Initialize(Action OnStartRoundMessageReceived,
                                                                    Action<int> OnWatchStateChanged,
                                                                    Action OnWatchTriggered);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_TakeControlWCSession();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_SyncRoundStartToWatch();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_SyncRoundResultToWatch(int roundResultIndex, int kill, float hitRate, float distance);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_QueryWatchState();

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnStartRoundMessageReceivedDelegate()
        {
            OnStartRoundMessageReceived?.Invoke();
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

        public static event Action OnStartRoundMessageReceived;

        public static event Action<MofaWatchState> OnWatchStateChanged;

        public static event Action OnWatchTriggered;

        public static void Initialize()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_Initialize(OnStartRoundMessageReceivedDelegate,
                                             OnWatchStateChangedDelegate,
                                             OnWatchTriggeredDelegate);
        }

        public static void TakeControlWatchConnectivitySession()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_TakeControlWCSession();
        }

        public static void SyncRoundStartToWatch()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_SyncRoundStartToWatch();
        }

        public static void SyncRoundResultToWatch(MofaIndividualRoundResult roundResult, int kill, float hitRate, float distance)
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_SyncRoundResultToWatch((int)roundResult, kill, hitRate, distance);
        }

        public static void QueryWatchState()
        {
            if (HoloKit.HoloKitUtils.IsEditor) { return; }
            MofaWatchConnectivity_QueryWatchState();
        }
    }
}
