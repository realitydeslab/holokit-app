using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;

namespace Holoi.Mofa.Base
{
    public class MofaPlayer : NetworkBehaviour
    {
        public NetworkVariable<int> KillCount = new(0, NetworkVariableReadPermission.Everyone);

        public NetworkVariable<int> DeathCount = new(0, NetworkVariableReadPermission.Everyone);

        public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone);

        public LifeShield LifeShield;

        public override void OnNetworkSpawn()
        {
            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetPlayer(OwnerClientId, this);
        }
    }
}