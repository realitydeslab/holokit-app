using System;
using System.Collections;
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
        RoundResult = 4,
        // When the summary board merges
        RoundData = 5
    }

    /// <summary>
    /// Round result in third person view
    /// </summary>
    public enum MofaRoundResult
    {
        NotDetermined = 0,
        BlueTeamWins = 1,
        RedTeamWins = 2,
        Draw = 3
    }

    /// <summary>
    /// Round result in first person view
    /// </summary>
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

        public int Death;

        public int HitRate;

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

        /// <summary>
        /// MofaPlayer prefab
        /// </summary>
        [SerializeField] private MofaPlayer _mofaPlayerPrefab;

        [Header("MOFA Settings")]
        [Tooltip("The duration of the countdown phase")]
        public float CountdownDuration = 3f;

        [Tooltip("The duration of a single round")]
        public float RoundDuration = 80f;

        /// <summary>
        /// The current phase of the game.
        /// </summary>
        public NetworkVariable<MofaPhase> CurrentPhase = new(MofaPhase.Waiting, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The current round number of the game.
        /// </summary>
        public NetworkVariable<int> RoundCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// This event is called on all clients if the game phase changes.
        /// </summary>
        public static event Action<MofaPhase> OnMofaPhaseChanged;

        public static event Action<MofaRoundResult> OnReceivedRoundResult;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // We register this delegate here in order to track player hit count
            LifeShield.OnBeingHit += OnLifeShieldBeingHit;
            // We register this delegate here in order to track player kill and death
            LifeShield.OnBeingDestroyed += OnLifeShieldBeingDestroyed;
            CurrentPhase.OnValueChanged += OnMofaPhaseChanged_Server;
            CurrentPhase.OnValueChanged += OnMofaPhaseChanged_Client;
        }

        public override void OnNetworkDespawn()
        {
            LifeShield.OnBeingHit -= OnLifeShieldBeingHit;
            LifeShield.OnBeingDestroyed -= OnLifeShieldBeingDestroyed;
            CurrentPhase.OnValueChanged -= OnMofaPhaseChanged_Server;
            CurrentPhase.OnValueChanged -= OnMofaPhaseChanged_Client;
        }

        private void OnMofaPhaseChanged_Server(MofaPhase oldPhase, MofaPhase newPhase)
        {
            if (oldPhase == newPhase) return;

            if (newPhase == MofaPhase.RoundData)
            {
                if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
                {
                    ResetLocalPlayerReadyState();
                }
            }
        }

        private void OnMofaPhaseChanged_Client(MofaPhase oldPhase, MofaPhase newPhase)
        {
            if (oldPhase == newPhase) return;

            OnMofaPhaseChanged?.Invoke(newPhase);
        }

        public MofaPlayer GetMofaPlayer(ulong clientId)
        {
            var player = HoloKitApp.HoloKitApp.Instance.MultiplayerManager.PlayerDict[clientId];
            if (player == null) return null;
            return player.GetComponent<MofaPlayer>();
        }

        protected void SpawnLifeShield(ulong clientId)
        {
            MofaPlayer mofaPlayer = GetMofaPlayer(clientId);
            var lifeShieldPrefab = LifeShieldList.GetLifeShield(mofaPlayer.MagicSchool.Value);
            var lifeShield = Instantiate(lifeShieldPrefab);
            lifeShield.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            MofaPlayer player = GetMofaPlayer(lifeShield.OwnerClientId);
            player.LifeShield = lifeShield;

            lifeShield.transform.SetParent(player.transform);
            lifeShield.transform.localPosition = player.LifeShieldOffset;
            lifeShield.transform.localRotation = Quaternion.identity;
            lifeShield.transform.localScale = Vector3.one;
        }

        public abstract void TryStartRound();

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
            yield return new WaitForSeconds(3f);
            CurrentPhase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            CurrentPhase.Value = MofaPhase.RoundData;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnSpellServerRpc(int spellId, Vector3 clientCenterEyePosition,
            Quaternion clientCenterEyeRotation, ulong clientId)
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
                GetMofaPlayer(ownerClientId).Death.Value++;;
            }
        }

        // Host only
        private MofaRoundResult GetRoundResult()
        {
            if (!IsServer)
            {
                Debug.LogError("[MofaBaseRealityManager] Only the host can compute the round result");
                return MofaRoundResult.Draw;
            }

            int blueTeamScore = 0;
            int redTeamScore = 0;
            foreach (var mofaPlayer in PlayerDict.Values)
            {
                if (mofaPlayer.Team.Value == MofaTeam.Blue)
                {
                    blueTeamScore += mofaPlayer.Kill.Value;
                }
                else
                {
                    redTeamScore += mofaPlayer.Kill.Value;
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

        public MofaIndividualStats GetIndividualStats(MofaPlayer player = null)
        {
            if (player == null)
            {
                player = GetPlayer();
            }

            MofaIndividualStats stats = new();
            // Inividual round result
            var roundResult = _roundResult.Value;
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
            // Kill
            stats.Kill = player.Kill.Value;
            // Death
            stats.Death = player.Death.Value;
            // Hit rate
            stats.HitRate = Mathf.RoundToInt((float)player.HitCount.Value / player.CastCount.Value * 100);
            // Distance
            stats.Distance = Mathf.RoundToInt(player.Dist.Value * MofaUtils.MeterToFoot);
            // Calories
            stats.Energy = Mathf.RoundToInt(player.Energy.Value);

            return stats;
        }

        private void ResetLocalPlayerReadyState()
        {
            GetPlayer().Ready.Value = false;
        }
    }
}