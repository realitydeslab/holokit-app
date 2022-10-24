using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    /// <summary>
    /// At any moment of the game, the game is at one of the following state.
    /// </summary>
    public enum MofaPhase
    {
        // Before the first round
        Waiting = 0,
        // When all players get ready
        Countdown = 1,
        // When the round actually begins
        Fighting = 2,
        // When the round over UI merges
        RoundOver = 3,
        // When the round result is shown
        RoundResult = 4,
        // When the summary board merges
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

    /// <summary>
    /// Stats which will be shown on the summary board after each round.
    /// </summary>
    public struct MofaIndividualStats
    {
        public MofaIndividualRoundResult IndividualRoundResult;

        public int Kill;

        public float HitRate;

        public float Distance;
    }

    public abstract class MofaBaseRealityManager : RealityManager
    {
        [Header("MOFA Base")]
        [SerializeField] private MofaPlayer _mofaPlayerPrefab;

        [SerializeField] private LifeShield _lifeShieldPrefab;

        public SpellList SpellList;

        public MofaPhase CurrentPhase => _currentPhase.Value;

        private NetworkVariable<MofaPhase> _currentPhase = new(0, NetworkVariableReadPermission.Everyone);

        public int RoundCount => _roundCount.Value;

        private NetworkVariable<int> _roundCount => new(0, NetworkVariableReadPermission.Everyone);

        public Dictionary<ulong, MofaPlayer> Players => _players;

        private readonly Dictionary<ulong, MofaPlayer> _players = new();

        public static event Action<MofaPhase> OnPhaseChanged;

        protected virtual void Start()
        {
            // We need to respawn life shields when they are destroyed.
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

            _currentPhase.OnValueChanged += OnPhaseChangedFunc;

            if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                // Currently we only support 1 on 1, so the host is always blue and the other player is always red 
                SpawnPlayerServerRpc(HoloKitApp.HoloKitApp.Instance.IsHost ? MofaTeam.Blue : MofaTeam.Red);
            }
        }

        public override void OnNetworkDespawn()
        {
            _currentPhase.OnValueChanged -= OnPhaseChangedFunc;
        }

        // This delegate method will be called on every client.
        private void OnPhaseChangedFunc(MofaPhase oldValue, MofaPhase newValue)
        {
            OnPhaseChanged?.Invoke(newValue);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SpawnPlayerServerRpc(MofaTeam team, ServerRpcParams serverRpcParams = default)
        {
            SpawnPlayer(team, serverRpcParams.Receive.SenderClientId);
        }

        // Host only
        protected void SpawnPlayer(MofaTeam team, ulong ownerClientId)
        {
            var player = Instantiate(_mofaPlayerPrefab);
            player.Team.Value = team;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        public void SpawnLifeShield(ulong ownerClientId)
        {
            var lifeShield = Instantiate(_lifeShieldPrefab);
            lifeShield.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
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

        public MofaPlayer GetPlayer(ulong clientId = 0)
        {
            if (Players.ContainsKey(clientId))
            {
                return Players[clientId];
            }
            else
            {
                Debug.Log($"[MofaBaseRealityManager] There is no player with clientId: {clientId}");
                return null;
            }
        }

        // Host only
        public void OnPlayerReadyStateChanged()
        {
            foreach (var player in _players.Values)
            {
                if (!player.Ready.Value)
                {
                    return;
                }
            }
            // Everyone is ready
            StartRound();
        }

        // Host only
        public virtual void StartRound()
        {
            if (_currentPhase.Value != MofaPhase.Waiting && _currentPhase.Value != MofaPhase.RoundData)
            {
                Debug.Log($"[MofaBaseRealityManager] You cannot start round at the current phase: {_currentPhase.Value}");
                return;
            }
            StartCoroutine(StartRoundFlow());
        }

        // Host only
        protected IEnumerator StartRoundFlow()
        {
            _currentPhase.Value = MofaPhase.Countdown;
            _roundCount.Value++;
            RespawnAllLifeShields();
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(80f);
            _currentPhase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.RoundData;
        }

        // Host only
        private void RespawnAllLifeShields()
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
            Spell spell = SpellList.GetSpell(spellId);
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

        public MofaIndividualStats GetIndividualStats(MofaPlayer player)
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
    }
}