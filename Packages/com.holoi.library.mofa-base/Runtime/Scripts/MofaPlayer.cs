using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;

namespace Holoi.Mofa.Base
{
    public class MofaPlayer : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<int> KillCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> DeathCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public LifeShield LifeShield;

        public override void OnNetworkSpawn()
        {
            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetPlayer(OwnerClientId, this);
        }
    }
}