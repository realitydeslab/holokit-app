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
        NotDetermined = 0,
        BlueTeamWins = 1,
        RedTeamWins = 2,
        Draw = 3
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

        public SpellList SpellList;

        public LifeShieldList LifeShieldList;

        public MofaPhase CurrentPhase => _currentPhase.Value;

        private NetworkVariable<MofaPhase> _currentPhase = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public int RoundCount => _roundCount.Value;

        private NetworkVariable<int> _roundCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public MofaRoundResult RoundResult => _roundResult.Value;

        private NetworkVariable<MofaRoundResult> _roundResult = new(MofaRoundResult.NotDetermined, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public Dictionary<ulong, MofaPlayer> Players => _players;

        private readonly Dictionary<ulong, MofaPlayer> _players = new();

        private const int RoundDuration = 80;

        public static event Action<MofaPhase> OnPhaseChanged;

        public static event Action<MofaRoundResult> OnReceivedRoundResult;

        protected virtual void Start()
        {
            // We need to respawn life shields when they are destroyed.
            LifeShield.OnDead += OnLifeShieldDead;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _currentPhase.OnValueChanged += OnPhaseChangedFunc;
            _roundResult.OnValueChanged += OnRoundResultChangedFunc;

            if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                string tokenId = HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId;
                var magicSchoolTokenId = int.Parse(tokenId);
                // Currently we only support 1 on 1, so the host is always blue and the other player is always red 
                SpawnPlayerServerRpc(magicSchoolTokenId, HoloKitApp.HoloKitApp.Instance.IsHost ? MofaTeam.Blue : MofaTeam.Red);
            }
        }

        public override void OnNetworkDespawn()
        {
            _currentPhase.OnValueChanged -= OnPhaseChangedFunc;
            _roundResult.OnValueChanged -= OnRoundResultChangedFunc;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LifeShield.OnDead -= OnLifeShieldDead;
        }

        // This delegate method will be called on every client.
        private void OnPhaseChangedFunc(MofaPhase oldValue, MofaPhase newValue)
        {
            Debug.Log($"Mofa phase changed to: {newValue}");
            OnPhaseChanged?.Invoke(newValue);

            if (newValue == MofaPhase.RoundData)
            {
                ResetLocalPlayerReadyState();
            }
        }

        private void OnRoundResultChangedFunc(MofaRoundResult oldValue, MofaRoundResult newValue)
        {
            if (newValue == MofaRoundResult.NotDetermined)
            {
                return;
            }
            Debug.Log($"Round result: {newValue}");
            OnReceivedRoundResult?.Invoke(newValue);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SpawnPlayerServerRpc(int magicSchoolTokenId, MofaTeam team, ServerRpcParams serverRpcParams = default)
        {
            SpawnPlayer(magicSchoolTokenId, team, serverRpcParams.Receive.SenderClientId);
        }

        // Host only
        public void SpawnPlayer(int magicSchoolTokenId, MofaTeam team, ulong ownerClientId)
        {
            var player = Instantiate(_mofaPlayerPrefab);
            player.MagicSchoolTokenId.Value = magicSchoolTokenId;
            player.Team.Value = team;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        public void SpawnLifeShield(ulong ownerClientId)
        {
            var lifeShieldPrefab = LifeShieldList.GetLifeShield(_players[ownerClientId].MagicSchoolTokenId.Value.ToString());
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
            //if (IsServer)
            //{
                lifeShield.transform.SetParent(shieldOwner.transform);
                lifeShield.transform.localPosition = shieldOwner.LifeShieldOffset;
                lifeShield.transform.localRotation = Quaternion.identity;
                lifeShield.transform.localScale = Vector3.one;
            //}
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

        // Host only
        public void OnPlayerReadyStateChanged()
        {
            if (_players.Count < 2)
            {
                return;
            }

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
        protected virtual void StartRound()
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
            _roundResult.Value = MofaRoundResult.NotDetermined;
            RespawnAllLifeShields();
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(RoundDuration);
            _currentPhase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            _currentPhase.Value = MofaPhase.RoundResult;
            _roundResult.Value = GetRoundResult();
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
            var spellInstance = Instantiate(spell, position, rotation);
            spellInstance.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId);
        }

        private void OnLifeShieldDead(ulong attackerClientId, ulong ownerClientId)
        {
            if (IsServer)
            {
                // Update the score
                _players[attackerClientId].KillCount.Value++;
                _players[ownerClientId].DeathCount.Value++;
                // Respawn life shield
                StartCoroutine(RespawnLifeShield(ownerClientId));
            }
        }

        private IEnumerator RespawnLifeShield(ulong ownerClientId)
        {
            //yield return new WaitForSeconds(3f - LifeShield.DestroyDelay);
            yield return new WaitForSeconds(3f);
            SpawnLifeShield(ownerClientId);
        }

        // Host only
        private MofaRoundResult GetRoundResult()
        {
            if (!IsServer)
            {
                Debug.LogError("[MofaBaseRealityManager] Only the host can compute round result");
                return MofaRoundResult.Draw;
            }

            int blueTeamScore = 0;
            int redTeamScore = 0;
            foreach (var mofaPlayer in Players.Values)
            {
                if (mofaPlayer.Team.Value == MofaTeam.Blue)
                {
                    blueTeamScore += mofaPlayer.KillCount.Value;
                }
                else
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
            // Kills
            stats.Kill = player.KillCount.Value;
            // TODO: Hit rate and distance

            return stats;
        }

        private void ResetLocalPlayerReadyState()
        {
            GetPlayer().Ready.Value = false;
        }
    }
}