using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Netcode;

namespace Netcode.Transports.MultipeerConnectivity
{
    public class MultipeerConnectivityTransport : NetworkTransport
    {
        public override ulong ServerClientId => 0;

        /// <summary>
        /// If this value is set to true, the device will automatically advertise
        /// after starting host. Otherwise, you need to call StartAdvertising()
        /// manually to start advertise.
        /// </summary>
        public bool AutomaticAdvertising = true;

        /// <summary>
        /// If this value is set to true, the device will automatically browse
        /// nearby devices. Otherwise, you need to call StartBrowsing manullay
        /// to start browsing.
        /// </summary>
        public bool AutomaticBrowsing = true;

        public static string BundleId = null;

        private static MultipeerConnectivityTransport s_instance;

        public static bool IsEditor => Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool IsRuntime => Application.platform == RuntimePlatform.IPhonePlayer;

        [DllImport("__Internal")]
        private static extern void MPC_Initialize(Action<string> OnBrowserFoundPeer,
                                                  Action<string> OnBrowserLostPeer,
                                                  Action<string> OnAdvertiserReceivedInvitation,
                                                  Action<string> OnConnectingWithPeer,
                                                  Action<int, string> OnConnectedWithPeer,
                                                  Action<int, string> OnDisconnectedWithPeer,
                                                  Action<int, IntPtr, int> OnReceivedData);

        [DllImport("__Internal")]
        private static extern void MPC_StartAdvertising(string bundleId);

        [DllImport("__Internal")]
        private static extern void MPC_StartBrowsing(string bundleId);

        [DllImport("__Internal")]
        private static extern void MPC_StopAdvertising();

        [DllImport("__Internal")]
        private static extern void MPC_StopBrowsing();

        [DllImport("__Internal")]
        private static extern void MPC_Shutdown();

        [DllImport("__Internal")]
        private static extern void MPC_SendData(int transportID, byte[] data, int length, bool reliable);

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnBrowserFoundPeerDelegate(string peerName)
        {
            if (s_instance != null)
            {
                OnBrowserFoundPeer?.Invoke(peerName);
            } 
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnBrowserLostPeerDelegate(string peerName)
        {
            if (s_instance != null)
            {
                OnBrowserLostPeer?.Invoke(peerName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnAdvertiserReceivedInvitationDelegate(string peerName)
        {
            if (s_instance != null)
            {
                OnAdvertiserReceivedInvitation?.Invoke(peerName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnConnectingWithPeerDelegate(string peerName)
        {
            if (s_instance != null)
            {
                OnConnectingWithPeer?.Invoke(peerName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnConnectedWithPeerDelegate(int transportID, string peerName)
        {
            if (s_instance != null)
            {
                s_instance.InvokeOnTransportEvent(NetworkEvent.Connect, (ulong)transportID,
                    default, Time.realtimeSinceStartup);
                OnConnectedWithPeer?.Invoke((ulong)transportID, peerName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnDisconnectedWithPeerDelegate(int transportID, string peerName)
        {
            if (s_instance != null)
            {
                s_instance.InvokeOnTransportEvent(NetworkEvent.Disconnect, (ulong)transportID,
                   default, Time.realtimeSinceStartup);
                OnDisconnectedWithPeer?.Invoke((ulong)transportID, peerName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int, IntPtr, int>))]
        private static void OnReceivedData(int transportID, IntPtr dataPtr, int length)
        {
            if (s_instance != null)
            {
                byte[] data = new byte[length];
                Marshal.Copy(dataPtr, data, 0, length);
                s_instance.InvokeOnTransportEvent(NetworkEvent.Data, (ulong)transportID,
                    new ArraySegment<byte>(data, 0, length), Time.realtimeSinceStartup);
            }
        }

        public static event Action<string> OnBrowserFoundPeer;

        public static event Action<string> OnBrowserLostPeer;

        public static event Action<string> OnAdvertiserReceivedInvitation;

        public static event Action<string> OnConnectingWithPeer;

        public static event Action<ulong, string> OnConnectedWithPeer;

        public static event Action<ulong, string> OnDisconnectedWithPeer;

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
            MPC_Initialize(OnBrowserFoundPeerDelegate,
                           OnBrowserLostPeerDelegate,
                           OnAdvertiserReceivedInvitationDelegate,
                           OnConnectingWithPeerDelegate,
                           OnConnectedWithPeerDelegate,
                           OnDisconnectedWithPeerDelegate,
                           OnReceivedData);

            if (BundleId == null)
            {
                Debug.Log("[MPCTransport] Initialized without BundleId");
            }
            else
            {
                Debug.Log($"[MPCTransport] Initilized with BundleId: {BundleId}");
            }
        }

        public override bool StartServer()
        {
            if (AutomaticAdvertising)
                MPC_StartAdvertising(BundleId);
            return true;
        }

        public override bool StartClient()
        {
            if (AutomaticBrowsing)
                MPC_StartBrowsing(BundleId);
            return true;
        }

        public static void StartAdvertising()
        {
            if (IsRuntime)
                MPC_StartAdvertising(BundleId);
            else
                Debug.Log("[MPCTransport] Cannot advertise on the current platform.");
        }

        public static void StopAdvertising()
        {
            if (IsRuntime)
                MPC_StopAdvertising();
        }

        public static void StartBrowsing()
        {
            if (IsRuntime)
                MPC_StartBrowsing(BundleId);
            else
                Debug.Log("[MPCTransport] Cannot browse on the current platform.");
        }

        public static void StopBrowsing()
        {
            if (IsRuntime)
                MPC_StopBrowsing();
        }

        public override NetworkEvent PollEvent(out ulong transportId, out ArraySegment<byte> payload, out float receiveTime)
        {
            transportId = 0;
            payload = default;
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

        /// <summary>
        /// This function is called when a client tries to disconnect from the server.
        /// </summary>
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
            MPC_Shutdown();
            BundleId = null;
        }
    }
}