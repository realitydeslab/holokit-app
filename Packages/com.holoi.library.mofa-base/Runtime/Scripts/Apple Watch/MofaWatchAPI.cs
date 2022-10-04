using System;
using System.Runtime.InteropServices;

namespace Holoi.Library.MOFABase
{
    public enum MofaWatchInput
    {
        Basic = 0,
        ChangeToNothing = 1,
        ChangeToGround = 2
    }

    public static class MofaWatchAPI
    {
        [DllImport("__Internal")]
        private static extern void MofaWatch_Initialize(Action OnReceivedStartRoundMessage,
                                                        Action<int> OnReceivedWatchInput);

        [DllImport("__Internal")]
        private static extern void MofaWatch_QueryWatchState();

        [DllImport("__Internal")]
        private static extern void MofaWatch_SendRoundResultDataMessage(int roundResult, int kill, float hitRate);

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnReceivedStartRoundMessageFunc()
        {
            OnReceivedStartRoundMessage?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnReceivedWatchInputFunc(int watchInput)
        {
            OnReceivedWatchInput?.Invoke((MofaWatchInput)watchInput);
        }

        public static event Action OnReceivedStartRoundMessage;

        public static event Action<MofaWatchInput> OnReceivedWatchInput;

        public static void Initialize()
        {
            MofaWatch_Initialize(OnReceivedStartRoundMessageFunc,
                                 OnReceivedWatchInputFunc);
        }

        public static void QueryWatchState()
        {
            MofaWatch_QueryWatchState();
        }

        public static void SendRoundResultDataMessage(int roundResult, int kill, float hitRate)
        {
            MofaWatch_SendRoundResultDataMessage(roundResult, kill, hitRate);
        }
    }
}
