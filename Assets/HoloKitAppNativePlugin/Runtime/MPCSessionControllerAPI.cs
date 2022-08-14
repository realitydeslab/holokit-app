using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace HoloKitAppNativePlugin
{
    public static class MPCSessionControllerAPI
    {
        [DllImport("__Internal")]
        private static extern void MPC_RegisterDelegates(
            Action<string> OnBrowserFoundCorrectPeer,
            Action<string, string> OnReceivedSessionCode);

        [DllImport("__Internal")]
        private static extern void MPC_StartAdvertising(string password);

        [DllImport("__Internal")]
        private static extern void MPC_StartBrowsing(string password, string sessionCode);

        [DllImport("__Internal")]
        private static extern void MPC_StopAdvertising();

        [DllImport("__Internal")]
        private static extern void MPC_StopBrowsing();

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnBrowserFoundCorrectPeerDelegate(string peerName)
        {
            OnBrowserFoundCorrectPeer?.Invoke(peerName);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void OnReceivedSessionCodeDelegate(string sessionCode, string peerName)
        {
            OnReceivedSessionCode?.Invoke(sessionCode, peerName);
        }

        public static event Action<string> OnBrowserFoundCorrectPeer;

        public static event Action<string, string> OnReceivedSessionCode;

        public static void RegisterDelegates()
        {
            MPC_RegisterDelegates(
                OnBrowserFoundCorrectPeerDelegate,
                OnReceivedSessionCodeDelegate);
        }

        public static void StartAdvertising(string password)
        {
            MPC_StartAdvertising(password);
        }

        public static void StartBrowsing(string password, string sessionCode)
        {
            MPC_StartBrowsing(password, sessionCode);
        }

        public static void StopAdvertising()
        {
            MPC_StopAdvertising();
        }

        public static void StopBrowsing()
        {
            MPC_StopBrowsing();
        }
    }
}