using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Netcode;

namespace Netcode.Transports.MultipeerConnectivity
{
    public class MultipeerConnectivityTransport : NetworkTransport
    {
        public override ulong ServerClientId => 0;

        public bool AutomaticAdvertisement = true;

        private static MultipeerConnectivityTransport s_instance;

        [DllImport("__Internal")]
        private static extern void MPC_Initialize(Action<int> OnClientConnected,
                                                  Action OnConnectedToHost,
                                                  Action<int, IntPtr, int> OnReceivedData,
                                                  Action<int> OnClientDisconnected,
                                                  Action OnHostDisconnected);

        [DllImport("__Internal")]
        private static extern void MPC_StartAdvertising();

        [DllImport("__Internal")]
        private static extern void MPC_StartBrowsing();

        [DllImport("__Internal")]
        private static extern void MPC_StopAdvertising();

        [DllImport("__Internal")]
        private static extern void MPC_StopBrowsing();

        [DllImport("__Internal")]
        private static extern void MPC_Deinitialize();

        [DllImport("__Internal")]
        private static extern void MPC_SendData(int transportID, byte[] data, int length, bool reliable);

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnClientConnected(int transportID)
        {
            s_instance.InvokeOnTransportEvent(NetworkEvent.Connect, (ulong)transportID,
                default, Time.realtimeSinceStartup);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnConnectedToHost()
        {
            s_instance.InvokeOnTransportEvent(NetworkEvent.Connect, 0,
                default, Time.realtimeSinceStartup);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int, IntPtr, int>))]
        private static void OnReceivedData(int transportID, IntPtr dataPtr, int length)
        {
            byte[] data = new byte[length];
            Marshal.Copy(dataPtr, data, 0, length);
            s_instance.InvokeOnTransportEvent(NetworkEvent.Data, (ulong)transportID,
                new ArraySegment<byte>(data, 0, length), Time.realtimeSinceStartup);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnClientDisconnected(int transportID)
        {
            s_instance.InvokeOnTransportEvent(NetworkEvent.Disconnect, (ulong)transportID,
                default, Time.realtimeSinceStartup);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnHostDisconnected()
        {
            s_instance.InvokeOnTransportEvent(NetworkEvent.Disconnect, 0,
                default, Time.realtimeSinceStartup);
        }

        private void Awake()
        {
            s_instance = this;
        }

        private void OnDestroy()
        {
            s_instance = null;
        }

        public override void Initialize(NetworkManager networkManager)
        {
            MPC_Initialize(OnClientConnected,
                           OnConnectedToHost,
                           OnReceivedData,
                           OnClientDisconnected,
                           OnHostDisconnected);
        }

        public override bool StartServer()
        {
            if (AutomaticAdvertisement)
            {
                MPC_StartAdvertising();
            }
            return true;
        }

        public override bool StartClient()
        {
            MPC_StartBrowsing();
            return true;
        }

        public static void StartAdvertising()
        {
            MPC_StartAdvertising();
        }

        public static void StopAdvertising()
        {
            MPC_StopAdvertising();
        }

        public override NetworkEvent PollEvent(out ulong transportId, out ArraySegment<byte> payload, out float receiveTime)
        {
            transportId = 0;
            payload = new ArraySegment<byte>();
            receiveTime = Time.realtimeSinceStartup;
            return NetworkEvent.Nothing;
        }

        public override void Send(ulong transportId, ArraySegment<byte> data, NetworkDelivery networkDelivery)
        {
            MPC_SendData((int)transportId, data.Array, data.Count,
                !(networkDelivery == NetworkDelivery.Unreliable || networkDelivery == NetworkDelivery.UnreliableSequenced));
        }

        public override ulong GetCurrentRtt(ulong transportId)
        {
            return 0;
        }

        public override void DisconnectLocalClient()
        {
            Debug.Log("[MPCTransport] DisconnectLocalClient");
        }

        public override void DisconnectRemoteClient(ulong transportId)
        {
            Debug.Log($"[MPCTransport] DisconnectRemoteClient {transportId}");
        }

        public override void Shutdown()
        {
            Debug.Log("[MPCTransport] Shutdown");
            MPC_Deinitialize();
        }
    }
}