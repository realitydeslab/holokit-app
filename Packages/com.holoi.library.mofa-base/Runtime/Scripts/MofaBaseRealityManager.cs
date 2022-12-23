using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    /// <summary>
    /// At any given moment, the game is at one of the following state.
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
        RoundResult = 4
    }

    /// <summary>
    /// Round result in third person view
    /// </summary>
    public enum MofaGeneralRoundResult
    {
        BlueTeamWins = 0,
        RedTeamWins = 1,
        Draw = 2
    }

    /// <summary>
    /// Round result in first person view
    /// </summary>
    public enum MofaPersonalRoundResult
    {
        Victory = 0,
        Defeat = 1,
        Draw = 2
    }

    /// <summary>
    /// Stats which will be shown on the summary board after each round.
    /// </summary>
    public struct MofaPlayerStats
    {
        public MofaPersonalRoundResult PersonalRoundResult;

        public int Kill;

        public int Death;

        public float HitRate;

        public float Distance;

        public float Energy;
    }

    public abstract class MofaBaseRealityManager : RealityManager
    {
        [Header("References")]
        /// <summary>
        /// A reference to the spell list scriptable object
        /// </summary>
        public SpellList SpellList;

        /// <summary>
        /// A reference to the life shield list scriptable object
        /// </summary>
        public LifeShieldList LifeShieldList;

        /// <summary>
        /// A reference to the spell pool object
        /// </summary>
        public MofaSpellPool SpellPool;

        [Header("Settings")]
        [Tooltip("The duration of the countdown phase")]
        public float CountdownDuration = 3f;

        [Tooltip("The duration of a single round")]
        public float RoundDuration = 80f;

        public float RoundOverDuration = 3f;

        public float RoundResultDuration = 3f;

        [Header("Network Variables")]
        /// <summary>
        /// The current phase of the game.
        /// </summary>
        public NetworkVariable<MofaPhase> CurrentPhase = new(MofaPhase.Waiting, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The current round number of the game.
        /// </summary>
        public NetworkVariable<int> RoundCount = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public MofaPlayer HostMofaPlayer
        {
            get
            {
                var hostPlayer = HoloKitApp.HoloKitApp.Instance.MultiplayerManager.HostPlayer;
                if (hostPlayer == null)
                    return null;
                var hostMofaPlayer = hostPlayer as MofaPlayer;
                if (hostMofaPlayer.Team.Value == MofaTeam.None)
                    return null;
                return hostMofaPlayer;
            }
        }

        public MofaPlayer LocalMofaPlayer
        {
            get
            {
                var localPlayer = HoloKitApp.HoloKitApp.Instance.MultiplayerManager.LocalPlayer;
                if (localPlayer == null)
                    return null;
                var localMofaPlayer = localPlayer as MofaPlayer;
                if (localMofaPlayer.Team.Value == MofaTeam.None)
                    return null;
                return localMofaPlayer;
            }
        }

        /// <summary>
        /// The collection of all non-spectator MofaPlayers.
        /// This computation is a bit expensive. Please reference this value when you use it.
        /// </summary>
        public ICollection<MofaPlayer> MofaPlayerList => HoloKitApp.HoloKitApp.Instance.MultiplayerManager.PlayerList
                                                            .Where(t => t.PlayerType.Value == HoloKitAppPlayerType.Player)
                                                            .Select(t => t as MofaPlayer)
                                                            .ToList();

        public Dictionary<ulong, MofaPlayer> MofaPlayerDict => HoloKitApp.HoloKitApp.Instance.MultiplayerManager.PlayerDict
                                                            .Where(t => t.Value.PlayerType.Value == HoloKitAppPlayerType.Player)
                                                            .ToDictionary(t => t.Key, t => t.Value as MofaPlayer);

        /// <summary>
        /// This event is called on all clients if the game phase changes.
        /// </summary>
        public static event Action<MofaPhase> OnMofaPhaseChanged;

        public override void OnNetworkSpawn()
        {
            // We register this delegate here in order to track player hit count
            LifeShield.OnBeingHit += OnLifeShieldBeingHit;
            // We register this delegate here in order to track player kill and death
            LifeShield.OnBeingDestroyed += OnLifeShieldBeingDestroyed;
            MofaPlayer.OnMofaPlayerReadyChanged += OnMofaPlayerReadyChanged;
            CurrentPhase.OnValueChanged += OnMofaPhaseChanged_Server;
            CurrentPhase.OnValueChanged += OnMofaPhaseChanged_Client;
        }

        public override void OnNetworkDespawn()
        {
            LifeShield.OnBeingHit -= OnLifeShieldBeingHit;
            LifeShield.OnBeingDestroyed -= OnLifeShieldBeingDestroyed;
            MofaPlayer.OnMofaPlayerReadyChanged -= OnMofaPlayerReadyChanged;
            CurrentPhase.OnValueChanged -= OnMofaPhaseChanged_Server;
            CurrentPhase.OnValueChanged -= OnMofaPhaseChanged_Client;
        }

        private void OnMofaPhaseChanged_Server(MofaPhase oldPhase, MofaPhase newPhase)
        {
            if (!IsServer) return;

            if (oldPhase == newPhase) return;

            switch (newPhase)
            {
                case MofaPhase.Waiting:
                    RoundCount.Value++;
                    break;
                case MofaPhase.Countdown:
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    UpdateEstimatedHealthData();
                    break;
                case MofaPhase.RoundResult:
                    break;
            }
        }

        private void OnMofaPhaseChanged_Client(MofaPhase oldPhase, MofaPhase newPhase)
        {
            if (oldPhase == newPhase) return;

            switch (newPhase)
            {
                case MofaPhase.Waiting:
                    if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
                        LocalMofaPlayer.SetReadyServerRpc(false);
                    break;
                case MofaPhase.Countdown:
                    if (RoundCount.Value == 1)
                        InitializeSpellPool();
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    break;
                case MofaPhase.RoundResult:
                    break;
            }

            OnMofaPhaseChanged?.Invoke(newPhase);
        }

        private void InitializeSpellPool()
        {
            var mofaPlayerList = MofaPlayerList;
            foreach (var mofaPlayer in mofaPlayerList)
            {
                SpellPool.RegisterSpellsForPlayerWithMagicSchoolIndex(mofaPlayer.MagicSchoolIndex.Value);
            }
        }

        /// <summary>
        /// Get a MofaPlayer by a Netcode client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public MofaPlayer GetMofaPlayer(ulong clientId)
        {
            var playerDict = HoloKitApp.HoloKitApp.Instance.MultiplayerManager.PlayerDict;
            if (playerDict.ContainsKey(clientId))
            {
                var player = HoloKitApp.HoloKitApp.Instance.MultiplayerManager.PlayerDict[clientId];
                return player.GetComponent<MofaPlayer>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This function is called when the local player tries to get ready.
        /// </summary>
        public virtual void TryGetReady()
        {
            GetReady();
        }

        /// <summary>
        /// Get ready for local player.
        /// </summary>
        protected void GetReady()
        {
            if (CurrentPhase.Value == MofaPhase.Waiting)
                LocalMofaPlayer.SetReadyServerRpc(true);
            else
                Debug.Log($"[MofaBaseRealityManager] You should not call 'TryGetReady()' in the current MOFA phase {CurrentPhase.Value}");
        }

        private void OnMofaPlayerReadyChanged(MofaPlayer mofaPlayer)
        {
            if (!IsServer) return;

            if (mofaPlayer.Ready.Value)
            {
                var mofaPlayerList = MofaPlayerList;
                // We need at least one player in each team to start
                if ((mofaPlayerList.Any(t => t.Team.Value == MofaTeam.Blue) && mofaPlayerList.Any(t => t.Team.Value == MofaTeam.Red)) || HoloKit.HoloKitUtils.IsEditor)
                {
                    // If all players are ready
                    int readyPlayerCount = mofaPlayerList.Count(t => t.Ready.Value);
                    if (readyPlayerCount == mofaPlayerList.Count)
                    {
                        SetupRound();
                        StartRound();
                    }
                }
            }
        }

        /// <summary>
        /// This function is called right before round start.
        /// </summary>
        protected virtual void SetupRound()
        {
            // Spawn life shield for every mofa player
            SetupLifeShields();

            // Reset stats for all players
            var mofaPlayerList = MofaPlayerList;
            foreach (var mofaPlayer in mofaPlayerList)
            {
                mofaPlayer.ResetStats();
            }
        }

        protected void SetupLifeShields()
        {
            var mofaPlayerList = MofaPlayerList;
            foreach (var mofaPlayer in mofaPlayerList)
            {
                if (mofaPlayer.LifeShield == null)
                    SpawnLifeShield(mofaPlayer.OwnerClientId);
                else
                    mofaPlayer.LifeShield.Renovate();
            }
        }

        protected void SpawnLifeShield(ulong clientId)
        {
            MofaPlayer mofaPlayer = GetMofaPlayer(clientId);
            var lifeShieldPrefab = LifeShieldList.GetLifeShield(mofaPlayer.MagicSchoolIndex.Value);
            var lifeShield = Instantiate(lifeShieldPrefab);
            lifeShield.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            MofaPlayer player = GetMofaPlayer(lifeShield.OwnerClientId);
            player.LifeShield = lifeShield;

            if (IsServer)
                lifeShield.GetComponent<NetworkObject>().TrySetParent(player.transform, false);
        }

        protected virtual void StartRound()
        {
            StartCoroutine(StartBaseRoundFlow());
        }

        /// <summary>
        /// Call this function to start a single MOFA round. This function can only
        /// be called by the server.
        /// </summary>
        protected IEnumerator StartBaseRoundFlow()
        {
            CurrentPhase.Value = MofaPhase.Countdown;
            yield return new WaitForSeconds(CountdownDuration);
            CurrentPhase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(RoundDuration);
            CurrentPhase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(RoundOverDuration);
            CurrentPhase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(RoundResultDuration);
            CurrentPhase.Value = MofaPhase.Waiting;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnSpellServerRpc(int spellId, Vector3 clientCenterEyePosition, Quaternion clientCenterEyeRotation, ulong clientId)
        {
            Spell spell = SpellList.GetSpell(spellId);
            var position = clientCenterEyePosition + clientCenterEyeRotation * spell.SpawnOffset;
            var rotation = clientCenterEyeRotation;
            if (spell.PerpendicularToGround)
            {
                rotation = MofaUtils.GetHorizontalRotation(rotation);
            }
            NetworkObject no = SpellPool.GetSpell(spell.gameObject, position, rotation);
            no.SpawnWithOwnership(clientId);
            // Record this cast event on the server
            GetMofaPlayer(clientId).CastCount.Value++;
        }

        private void OnLifeShieldBeingHit(ulong attackerClientId, ulong ownerClientId)
        {
            if (IsServer)
                GetMofaPlayer(attackerClientId).HitCount.Value++;
        }

        private void OnLifeShieldBeingDestroyed(ulong attackerClientId, ulong ownerClientId)
        {
            if (IsServer)
            {
                // Update the score
                GetMofaPlayer(attackerClientId).Kill.Value++;
                GetMofaPlayer(ownerClientId).Death.Value++;
            }
        }

        public MofaGeneralRoundResult GetGeneralRoundResult()
        {
            var mofaPlayerList = MofaPlayerList;
            // Calculate scores
            int blueTeamScore = mofaPlayerList.Where(t => t.Team.Value == MofaTeam.Blue).Sum(t => t.Kill.Value);
            int redTeamScore = mofaPlayerList.Where(t => t.Team.Value == MofaTeam.Red).Sum(t => t.Kill.Value);
            if (blueTeamScore == redTeamScore)
                return MofaGeneralRoundResult.Draw;
            return blueTeamScore > redTeamScore ? MofaGeneralRoundResult.BlueTeamWins : MofaGeneralRoundResult.RedTeamWins;
        }

        public MofaPersonalRoundResult GetPersonalRoundResult(MofaPlayer mofaPlayer)
        {
            var generalRoundResult = GetGeneralRoundResult();
            switch (generalRoundResult)
            {
                case MofaGeneralRoundResult.BlueTeamWins:
                    return mofaPlayer.Team.Value == MofaTeam.Blue ? MofaPersonalRoundResult.Victory : MofaPersonalRoundResult.Defeat;
                case MofaGeneralRoundResult.RedTeamWins:
                    return mofaPlayer.Team.Value == MofaTeam.Red ? MofaPersonalRoundResult.Victory : MofaPersonalRoundResult.Defeat;
                default:
                    return MofaPersonalRoundResult.Draw;
            }
        }

        public MofaPlayerStats GetPlayerStats(MofaPlayer mofaPlayer)
        {
            MofaPlayerStats stats = new();
            stats.PersonalRoundResult = GetPersonalRoundResult(mofaPlayer);
            stats.Kill = mofaPlayer.Kill.Value;
            stats.Death = mofaPlayer.Death.Value;
            stats.HitRate = (float)mofaPlayer.HitCount.Value / mofaPlayer.CastCount.Value;
            stats.Distance = mofaPlayer.Distance.Value * MofaUtils.MeterToFoot;
            stats.Energy = mofaPlayer.Energy.Value;

            return stats;
        }

        /// <summary>
        /// Calculate the estimated distance and energy for each players. These estimated
        /// values will be overridden when the actual HealthKit data is received from the
        /// Apple Watch.
        /// </summary>
        public void UpdateEstimatedHealthData()
        {
            var mofaPlayerList = MofaPlayerList;
            foreach (var mofaPlayer in mofaPlayerList)
            {
                mofaPlayer.Distance.Value = mofaPlayer.AltDistance;
                mofaPlayer.Energy.Value = mofaPlayer.AltDistance * MofaUtils.MeterToKcal;
            }
        }
    }
}