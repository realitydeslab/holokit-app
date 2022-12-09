using System;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public partial class HoloKitAppMultiplayerManager : NetworkBehaviour
    {
        public static event Action OnLocalClientConnected;

        private void Start()
        {
            Start_Host();
        }

        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetMultiplayerManager(this);
            OnLocalClientConnected?.Invoke();
            OnNetworkSpawn_Client();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnDestroy_Host();
        }

        private void FixedUpdate()
        {
            FixedUpdate_Client();
        }
    }
}
