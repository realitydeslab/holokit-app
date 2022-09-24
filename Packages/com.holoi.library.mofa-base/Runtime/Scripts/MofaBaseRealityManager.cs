using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using System;
using HoloKit;

namespace Holoi.Mofa.Base
{
    public enum MofaPhase
    {
        Waiting = 0,
        Countdown = 1,
        Fighting = 2,
        RoundOver = 3,
        RoundResult = 4,
        RoundData = 5
    }

    public enum MofaRoundResult
    {
        BlueTeamWins,
        RedTeamWins,
        Draw
    }

    public abstract class MofaBaseRealityManager : RealityManager
    {
        [HideInInspector] public NetworkVariable<MofaPhase> Phase = new(0, NetworkVariableReadPermission.Everyone);

        public MofaPlayer MofaPlayerPrefab;

        public LifeShield LifeShieldPrefab;

        public LocalPlayerSpellManager LocalPlayerSpellManagerPrefab;

        [HideInInspector] public LocalPlayerSpellManager LocalPlayerSpellManager;

        public Dictionary<ulong, MofaPlayer> Players = new();

        public static event Action<MofaPhase> OnPhaseChanged;

        protected override void Awake()
        {
            base.Awake();
            LifeShield.OnDead += OnLifeShieldDead;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LifeShield.OnDead -= OnLifeShieldDead;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Phase.OnValueChanged += OnPhaseChangedFunc;
        }

        public override void OnNetworkDespawn()
        {
            Phase.OnValueChanged -= OnPhaseChangedFunc;
        }

        private void OnPhaseChangedFunc(MofaPhase oldValue, MofaPhase newValue)
        {
            Debug.Log($"[MOFABase] Phase changed to {newValue}");
            OnPhaseChanged?.Invoke(newValue);
        }

        protected void SpawnLocalPlayerSpellManager()
        {
            LocalPlayerSpellManager = Instantiate(LocalPlayerSpellManagerPrefab);
            LocalPlayerSpellManager.transform.SetParent(transform);
        }

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
            lifeShield.transform.localPosition = shieldOwner.LifeShieldOffest;
            lifeShield.transform.localRotation = Quaternion.identity;
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

        protected IEnumerator StartSingleRound()
        {
            Phase.Value = MofaPhase.Countdown;
            RespawnEachPlayersLifeShield();
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(80f);
            Phase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.RoundData;
        }

        private void RespawnEachPlayersLifeShield()
        {
            foreach (var clientId in Players.Keys)
            {
                var lifeShield = Players[clientId].LifeShield;
                if (lifeShield != null)
                {
                    Destroy(lifeShield.gameObject);
                }
                SpawnLifeShield(clientId);
            }
        }

        [ServerRpc]
        public void SpawnSpellServerRpc(int spellId, Vector3 clientCenterEyePosition,
            Quaternion clientCenterEyeRotation, ulong ownerClientId)
        {
            Spell spell = LocalPlayerSpellManager.SpellList.List[spellId];
            var position = clientCenterEyePosition + clientCenterEyeRotation * spell.SpawnOffset;
            var rotation = clientCenterEyeRotation;
            if (spell.PerpendicularToGround)
            {
                rotation = MofaUtils.GetHorizontalRotation(rotation);
            }
            var spellInstance = Instantiate(spell, position, rotation);
            spellInstance.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        private void OnLifeShieldDead(ulong ownerClientId)
        {
            if (IsServer)
            {
                StartCoroutine(RespawnLifeShield(ownerClientId));
            }
        }

        private IEnumerator RespawnLifeShield(ulong ownerClientId)
        {
            yield return new WaitForSeconds(3f - LifeShield.DestroyDelay);
            SpawnLifeShield(ownerClientId);
        }

        public int GetTeamScore(MofaTeam team)
        {
            int teamScore = 0;
            foreach (var mofaPlayer in Players.Values)
            {
                if (mofaPlayer.Team.Value == team)
                {
                    teamScore += mofaPlayer.KillCount.Value;
                }
            }
            return teamScore;
        }

        public MofaRoundResult GetRoundResult()
        {
            int blueTeamScore = 0;
            int redTeamScore = 0;
            foreach (var mofaPlayer in Players.Values)
            {
                if (mofaPlayer.Team.Value == MofaTeam.Blue)
                {
                    blueTeamScore += mofaPlayer.KillCount.Value;
                }
                else // Red
                {
                    redTeamScore += mofaPlayer.KillCount.Value;
                }
            }

            if (blueTeamScore > redTeamScore)
            {
                return MofaRoundResult.BlueTeamWins;
            }
            else if (redTeamScore > blueTeamScore)
            {
                return MofaRoundResult.RedTeamWins;
            }
            else
            {
                return MofaRoundResult.Draw;
            }
        }

        public MofaPlayer GetLocalPlayer()
        {
            if (Players.ContainsKey(NetworkManager.LocalClientId))
            {
                return Players[NetworkManager.LocalClientId];
            }
            else
            {
                return null;
            }
        }

        public bool IsLocalPlayerSpectator()
        {
            return !Players.ContainsKey(NetworkManager.LocalClientId);
        }
    }
}