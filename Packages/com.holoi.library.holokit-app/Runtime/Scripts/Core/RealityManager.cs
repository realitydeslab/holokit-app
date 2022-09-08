using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;
using Netcode.Transports.MultipeerConnectivity;

namespace Holoi.HoloKit.App
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log("[RealityManager] OnNetworkSpawn");
            HoloKitApp.Instance.SetRealityManager(this);
        }

        public void StartSharingReality()
        {
            MultipeerConnectivityTransport.StartAdvertising();
        }

        public void StopSharingReality()
        {
            MultipeerConnectivityTransport.StopAdvertising();
        }
    }
}