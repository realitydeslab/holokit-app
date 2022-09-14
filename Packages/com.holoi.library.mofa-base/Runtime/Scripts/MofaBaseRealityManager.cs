using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;
using System;

namespace Holoi.Mofa.Base
{
    public class MofaBaseRealityManager : RealityManager
    {
        [HideInInspector] public NetworkVariable<bool> IsFighting = new(false, NetworkVariableReadPermission.Everyone);

        public MofaPlayer MofaPlayerPrefab;

        public LifeShield LifeShieldPrefab;

        public Dictionary<ulong, MofaPlayer> Players = new();

        public static event Action OnCoutdownStarted;

        public static event Action OnRoundStarted;

        public static event Action OnRoundEnded;

        public static event Action OnRoundResultDisplayed;

        public static event Action OnRoundDataDisplayed;

        public void SetPlayer(ulong clientId, MofaPlayer mofaPlayer)
        {
            Players[clientId] = mofaPlayer;
            mofaPlayer.transform.SetParent(transform);
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            MofaPlayer shieldOwner = Players[lifeShield.OwnerClientId];
            shieldOwner.LifeShield = lifeShield;
            lifeShield.transform.SetParent(shieldOwner.transform);
        }

        public void SpawnMofaPlayer(MofaTeam team, ulong ownerClientId)
        {
            var player = Instantiate(MofaPlayerPrefab);
            player.Team.Value = team;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        public void SpawnLifeShield(ulong ownerClientId)
        {
            var lifeShield = Instantiate(LifeShieldPrefab);
            lifeShield.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        public void StartRound()
        {

        }
    }
}