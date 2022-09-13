using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;

namespace Holoi.Mofa.Base
{
    public enum MofaTeam
    {
        Blue = 0,
        Red = 1,
        Spectator = 2
    }

    public class MofaPlayer : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<MofaTeam> Team = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> KillCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> DeathCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public LifeShield LifeShield;

        public override void OnNetworkSpawn()
        {
            Debug.Log($"[MofaPlayer] OnNetworkSpawned with ownership {OwnerClientId} and team {Team.Value}");

            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetPlayer(OwnerClientId, this);
        }
    }
}