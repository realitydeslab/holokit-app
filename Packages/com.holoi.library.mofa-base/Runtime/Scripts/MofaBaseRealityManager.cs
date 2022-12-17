using System;
using System.Collections;
using System.Collections.Generic;
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

        public MofaPhase CurrentPhase
        {
            get => _currentPhase.Value;
            set
            {
                _currentPhase.Value = value;
            }
        }

        public int RoundCount => _roundCount.Value;

        public MofaRoundResult RoundResult
        {
            get => _roundResult.Value;
            set
            {
                _roundResult.Value = value;
            }
        }

        public Dictionary<ulong, MofaPlayer> Players => _players;

        /// <summary>
        /// The current phase of the game.
        /// </summary>
        private readonly NetworkVariable<MofaPhase> _currentPhase = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The current round number of the game.
        /// </summary>
        private readonly NetworkVariable<int> _roundCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The result of the last round.
        /// </summary>
        private readonly NetworkVariable<MofaRoundResult> _roundResult = new(MofaRoundResult.NotDetermined, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The dictionary stores all the connected MOFA players
        /// </summary>
        private readonly Dictionary<ulong, MofaPlayer> _players = new();

        /// <summary>
        /// This event is called on all clients if the game phase changes.
        /// </summary>
        public static event Action<MofaPhase> OnMofaPhaseChanged;

        public static event Action<MofaRoundResult> OnReceivedRoundResult;

        protected virtual void Start()
        {
            // We register this delegate here in order to track player hit count
            LifeShield.OnBeingHit += OnLifeShieldBeingHit;
            // We register this delegate here in order to track player kill and death
            LifeShield.OnBeingDestroyed += OnLifeShieldBeingDestroyed;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _currentPhase.OnValueChanged += OnMofaPhaseChangedFunc;
            _roundResult.OnValueChanged += OnRoundResultChangedFunc;

            if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                string tokenId = HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId;
                var magicSchoolTokenId = int.Parse(tokenId);
                // Currently we only support 1 on 1, so the host is always blue and the other player is always red 
                SpawnMofaPlayerServerRpc(magicSchoolTokenId, HoloKitApp.HoloKitApp.Instance.IsHost ? MofaTeam.Blue : MofaTeam.Red);
            }
        }

        public override void OnNetworkDespawn()
        {
            _currentPhase.OnValueChanged -= OnMofaPhaseChangedFunc;
            _roundResult.OnValueChanged -= OnRoundResultChangedFunc;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LifeShield.OnBeingHit -= OnLifeShieldBeingHit;
            LifeShield.OnBeingDestroyed -= OnLifeShieldBeingDestroyed;
        }

        /// <summary>
        /// This delegate function is called on every client when mofa phase changes.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void OnMofaPhaseChangedFunc(MofaPhase oldValue, MofaPhase newValue)
        {
            if (oldValue == newValue) return;

            OnMofaPhaseChanged?.Invoke(newValue);
            if (newValue == MofaPhase.RoundData)
            {
                if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
                {
                    ResetLocalPlayerReadyState();
                }
            }
        }

        private void OnRoundResultChangedFunc(MofaRoundResult oldValue, MofaRoundResult newValue)
        {
            if (newValue == MofaRoundResult.NotDetermined) return;
            Debug.Log($"Round result: {newValue}");
            OnReceivedRoundResult?.Invoke(newValue);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SpawnMofaPlayerServerRpc(int magicSchoolTokenId, MofaTeam team, ServerRpcParams serverRpcParams = default)
        {
            SpawnMofaPlayer(magicSchoolTokenId, team, serverRpcParams.Receive.SenderClientId);
        }

        // Host only
        public void SpawnMofaPlayer(int magicSchoolTokenId, MofaTeam team, ulong ownerClientId)
        {
            var player = Instantiate(_mofaPlayerPrefab);
            player.MagicSchoolTokenId.Value = magicSchoolTokenId;
            player.Team.Value = team;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        protected void SpawnLifeShield(ulong ownerClientId)
        {
            var lifeShieldPrefab = LifeShieldList.GetLifeShield(_players[ownerClientId].MagicSchoolTokenId.Value);
            var lifeShield = Instantiate(lifeShieldPrefab);
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

            lifeShield.transform.SetParent(shieldOwner.transform);
            lifeShield.transform.localPosition = shieldOwner.LifeShieldOffset;
            lifeShield.transform.localRotation = Quaternion.identity;
            lifeShield.transform.localScale = Vector3.one;
        }

        public MofaPlayer GetPlayer(ulong clientId = 0)
        {
            if (!HoloKitApp.HoloKitApp.Instance.IsHost)
            {
                clientId = NetworkManager.LocalClientId;
            }

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

        public abstract void TryStartRound();

        // Host only
        protected virtual IEnumerator StartRoundFlow()
        {
            PutTheArmorOn();
            _currentPhase.Value = MofaPhase.Countdown;
            _roundCount.Value++;
            _roundResult.Value = MofaRoundResult.NotDetermined;
            yield return new WaitForSeconds(CountdownDuration);
            _currentPhase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(RoundDuration);
            _currentPhase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            _roundResult.Value = GetRoundResult();
            _currentPhase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.RoundData;
        }

        /// <summary>
        /// Spawn life shield for every player.
        /// </summary>
        private void PutTheArmorOn()
        {
            foreach (var clientId in Players.Keys)
            {
                if (Players[clientId].LifeShield == null)
                {
                    SpawnLifeShield(clientId);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
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
            NetworkObject no = SpellPool.GetSpell(spell.gameObject, position, rotation);
            no.SpawnWithOwnership(ownerClientId);
            // Record this cast event on the server
            _players[ownerClientId].CastCount.Value++;
        }

        private void OnLifeShieldBeingHit(ulong attackerClientId, ulong ownerClientId)
        {
            if (IsServer)
            {
                _players[attackerClientId].HitCount.Value++;
            }
        }

        private void OnLifeShieldBeingDestroyed(ulong attackerClientId, ulong ownerClientId)
        {
            if (IsServer)
            {
                // Update the score
                _players[attackerClientId].Kill.Value++;
                _players[ownerClientId].Death.Value++;;
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
            foreach (var mofaPlayer in Players.Values)
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
            stats.Distance = Mathf.RoundToInt(player.Distance.Value * MofaUtils.MeterToFoot);
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