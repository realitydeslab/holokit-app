using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public string SceneName;

        public List<GameObject> NetworkPrefabs;

        public static event Action<RealityManager> OnRealityManagerSpawned;

        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.SetRealityManager(this);
            OnRealityManagerSpawned?.Invoke(this);
        }
    }
}