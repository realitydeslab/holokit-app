using System;
using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp.WatchConnectivity.MOFA
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
            OnReceivedHealthDataMessage?.Invoke(dist, energy);
        }

        public static event Action OnReceivedStartRoundMessage;

        public static event Action<MofaWatchState> OnWatchStateChanged;

        public static event Action OnWatchTriggered;

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
