using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.WatchConnectivity;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
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
        BlueTeamWins = 0,
        RedTeamWins = 1,
        Draw = 2
    }

    public enum MofaIndividualRoundResult
    {
        Victory = 0,
        Defeat = 1,
        Draw = 2
    }

    public struct MofaIndividualStats
    {
        public MofaIndividualRoundResult IndividualRoundResult;

        public int Kill;

        public float HitRate;

        public float Distance;
    }

    public abstract class MofaBaseRealityManager : RealityManager
    {
        [HideInInspector] public NetworkVariable<MofaPhase> Phase = new(0, NetworkVariableReadPermission.Everyone);

        [Header("MOFA Base")]
        public MofaPlayer MofaPlayerPrefab;

        public LifeShield LifeShieldPrefab;

        public LocalPlayerSpellManager LocalPlayerSpellManagerPrefab;

        [HideInInspector] public LocalPlayerSpellManager LocalPlayerSpellManager;

        public Dictionary<ulong, MofaPlayer> Players = new();

        public int RoundCount = 0;

        public static event Action<MofaPhase> OnPhaseChanged;

        protected virtual void Awake()
        {
            LifeShield.OnDead += OnLifeShieldDead;

            // Apple Watch
            MofaWatchConnectivityAPI.Initialize();
            // MofaWatchConnectivityManager should take control first
            MofaWatchConnectivityAPI.TakeControlWatchConnectivitySession();
            // We then update the control on Watch side so that MofaWatchConnectivityManager won't miss messages.
            HoloKitAppWatchConnectivityAPI.UpdateCurrentReality(WatchReality.MOFATheTraining);
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

        // This delegate method will be called on every client.
        private void OnPhaseChangedFunc(MofaPhase oldValue, MofaPhase newValue)
        {
            Debug.Log($"[MOFABase] Phase changed to {newValue}");
            OnPhaseChanged?.Invoke(newValue);

            switch (newValue)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    MofaWatchConnectivityAPI.SyncRoundStartToWatch();
                    RoundCount++;
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    break;
                case MofaPhase.RoundResult:
                    if (!IsLocalPlayerSpectator())
                    {
                        var localPlayerStats = GetIndividualStats(GetLocalPlayer());
                        MofaWatchConnectivityAPI.SyncRoundResultToWatch(localPlayerStats.IndividualRoundResult,
                                                                        localPlayerStats.Kill,
                                                                        localPlayerStats.HitRate,
                                                                        localPlayerStats.Distance);
                    }
                    break;
                case MofaPhase.RoundData:
                    break;
            }
        }

        protected void SpawnLocalPlayerSpellManager()
        {
            LocalPlayerSpellManager = Instantiate(LocalPlayerSpellManagerPrefab);
            LocalPlayerSpellManager.transform.SetParent(transform);
        }

        public void SetPlayer(ulong clientId, MofaPlayer mofaPlayer)
        {
            Players[clientId] = mofaPlayer;
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            MofaPlayer shieldOwner = Players[lifeShield.OwnerClientId];
            shieldOwner.LifeShield = lifeShield;
            if (IsServer)
            {
                lifeShield.transform.SetParent(shieldOwner.transform);
                lifeShield.transform.localPosition = shieldOwner.LifeShieldOffest;
                lifeShield.transform.localRotation = Quaternion.identity;
            }
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
            Spell spell = LocalPlayerSpellManager.SpellList.GetSpell(spellId);
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

        private MofaIndividualStats GetIndividualStats(MofaPlayer player)
        {
            MofaIndividualStats stats = new();
            // Inividual round result
            var roundResult = GetRoundResult();
            if (roundResult == MofaRoundResult.Draw)
            {
                stats.IndividualRoundResult = MofaIndividualRoundResult.Draw;
            }
            else
            {
                if (player.Team.Value == MofaTeam.Blue)
                {

                    stats.IndividualRoundResult = roundResult == MofaRoundResult.BlueTeamWins ?
                        MofaIndividualRoundResult.Victory : MofaIndividualRoundResult.Defeat;
                }
                else
                {
                    stats.IndividualRoundResult = roundResult == MofaRoundResult.RedTeamWins ?
                        MofaIndividualRoundResult.Victory : MofaIndividualRoundResult.Defeat;
                }
            }
            // Kills
            stats.Kill = player.KillCount.Value;
            // TODO: Hit rate and distance

            return stats;
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

        public virtual void StartRound()
        {
            if (Phase.Value != MofaPhase.Waiting && Phase.Value != MofaPhase.RoundData)
            {
                return;
            }
        }
    }
}