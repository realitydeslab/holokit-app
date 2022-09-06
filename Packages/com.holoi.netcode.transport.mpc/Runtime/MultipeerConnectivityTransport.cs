using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Netcode;

namespace Netcode.Transports.MultipeerConnectivity
{
    public struct DataPacket
    {
        public int TransportID;
        public byte[] Data;
        public int Length;
    }

    public class MultipeerConnectivityTransport : NetworkTransport
    {
        public override ulong ServerClientId => 0;

        public bool AutomaticAdvertisement = true;

        private static bool s_isHost;

        private static bool s_clientConnected;

        private static List<int> s_connectedClientList = new();

        private static bool s_connectedToHost;

        private static bool s_clientDisconnected;

        private static int s_disconnectedClientTransportID;

        private static bool s_hostDisconnected;

        private static readonly Queue<DataPacket> s_dataPacketQueue = new();

        [DllImport("__Internal")]
        private static extern void MPC_Initialize(Action<int> OnClientConnected,
                                                  Action OnConnectedToHost,
                                                  Action<int, IntPtr, int> OnReceivedData,
                                                  Action<int> OnClientDisconnected,
                                                  Action OnHostDisconnected);

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnClientConnected(int transportID)
        {
            Debug.Log($"[MPCTransport] OnClientConnected {transportID}");
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnConnectedToHost()
        {
            Debug.Log($"[MPCTransport] OnConnectedToHost");
            s_connectedToHost = true;
        }

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

        [AOT.MonoPInvokeCallback(typeof(Action<int, IntPtr, int>))]
        private static void OnReceivedData(int transportID, IntPtr dataPtr, int length)
        {
            if (s_isHost && !s_connectedClientList.Contains(transportID))
            {
                s_connectedClientList.Add(transportID);
                s_clientConnected = true;
            }
            byte[] data = new byte[length];
            Marshal.Copy(dataPtr, data, 0, length);
            DataPacket dataPacket = new DataPacket()
            {
                TransportID = transportID,
                Data = data,
                Length = length
            };
            s_dataPacketQueue.Enqueue(dataPacket);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnClientDisconnected(int transportID)
        {
            Debug.Log($"[MPCTransport] OnClientDisconnected {transportID}");
            s_disconnectedClientTransportID = transportID;
            s_clientDisconnected = true;
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnHostDisconnected()
        {
            Debug.Log("[MPCTransport] OnHostDisconnected");
            s_hostDisconnected = true;
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
            s_isHost = true;
            return true;
        }

        public override bool StartClient()
        {
            MPC_StartBrowsing();
            s_isHost = false;
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
            if (s_connectedToHost)
            {
                s_connectedToHost = false;
                transportId = 0;
                payload = new ArraySegment<byte>();
                receiveTime = Time.realtimeSinceStartup;
                return NetworkEvent.Connect;
            }

            if (s_clientConnected)
            {
                s_clientConnected = false;
                transportId = (ulong)s_connectedClientList[^1];
                payload = new ArraySegment<byte>();
                receiveTime = Time.realtimeSinceStartup;
                return NetworkEvent.Connect;
            }

            if (s_dataPacketQueue.Count > 0)
            {
                DataPacket dataPacket = s_dataPacketQueue.Dequeue();
                transportId = (ulong)dataPacket.TransportID;
                payload = new ArraySegment<byte>(dataPacket.Data, 0, dataPacket.Length);
                receiveTime = Time.realtimeSinceStartup;
                return NetworkEvent.Data;
            }

            if (s_clientDisconnected)
            {
                s_clientDisconnected = false;
                transportId = (ulong)s_disconnectedClientTransportID;
                payload = new ArraySegment<byte>();
                receiveTime = Time.realtimeSinceStartup;
                return NetworkEvent.Disconnect;
            }

            if (s_hostDisconnected)
            {
                s_hostDisconnected = false;
                transportId = 0;
                payload = new ArraySegment<byte>();
                receiveTime = Time.realtimeSinceStartup;
                return NetworkEvent.Disconnect;
            }

            transportId = 0;
            payload = new ArraySegment<byte>();
            receiveTime = Time.realtimeSinceStartup;
            return NetworkEvent.Nothing;
        }

        public override void Send(ulong transportId, ArraySegment<byte> data, NetworkDelivery networkDelivery)
        {
            byte[] arr = new byte[data.Count];
            Array.Copy(data.Array, data.Offset, arr, 0, data.Count);
            MPC_SendData((int)transportId, arr, data.Count,
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
            s_clientConnected = false;
            s_connectedClientList.Clear();
            s_connectedToHost = false;
            s_dataPacketQueue.Clear();
            MPC_Deinitialize();
        }
    }
}