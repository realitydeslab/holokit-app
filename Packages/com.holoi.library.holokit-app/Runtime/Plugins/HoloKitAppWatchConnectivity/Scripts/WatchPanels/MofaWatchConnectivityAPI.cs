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
        private static extern void MofaWatchConnectivity_UpdateMagicSchool(int magicSchool);

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_OnRoundStarted();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_QueryWatchState();

        [DllImport("__Internal")]
        private static extern void MofaWatchConnectivity_OnRoundEnded(int result, int kill, int hitRate);

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
            if (HoloKit.HoloKitUtils.IsEditor) return;

            MofaWatchConnectivity_Initialize(OnReceivedStartRoundMessageDelegate,
                                             OnWatchStateChangedDelegate,
                                             OnWatchTriggeredDelegate,
                                             OnReceivedHealthDataMessageDelegate);
        }

        public static void UpdateMagicSchool(int magicSchool)
        {
            if (HoloKit.HoloKitUtils.IsEditor) return;

            MofaWatchConnectivity_UpdateMagicSchool(magicSchool);
        }

        public static void OnRoundStarted()
        {
            if (HoloKit.HoloKitUtils.IsEditor) return;

            MofaWatchConnectivity_OnRoundStarted();
        }

        public static void QueryWatchState()
        {
            if (HoloKit.HoloKitUtils.IsEditor) return;

            MofaWatchConnectivity_QueryWatchState();
        }

        public static void OnRoundEnded(int result, int kill, int hitRate)
        {
            if (HoloKit.HoloKitUtils.IsEditor) return;

            MofaWatchConnectivity_OnRoundEnded(result, kill, hitRate);
        }
    }
}
