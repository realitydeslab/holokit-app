using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;

namespace Holoi.HoloKit.App
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public string SceneName;

        public List<GameObject> NetworkPrefabs;

        public override void OnNetworkSpawn()
        {
            //Debug.Log("[RealityManager] OnNetworkSpawn");
            HoloKitApp.Instance.SetRealityManager(this);
        }
    }
}