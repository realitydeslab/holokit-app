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

    public abstract class MofaBaseRealityManager : RealityManager
    {
        [HideInInspector] public NetworkVariable<MofaPhase> Phase = new(0, NetworkVariableReadPermission.Everyone);

        public MofaPlayer MofaPlayerPrefab;

        public LifeShield LifeShieldPrefab;

        public LocalPlayerSpellManager LocalPlayerSpellManagerPrefab;

        [HideInInspector] public LocalPlayerSpellManager LocalPlayerSpellManager;

        public Dictionary<ulong, MofaPlayer> Players = new();

        public static event Action OnCoutdownStarted;

        public static event Action OnRoundStarted;

        public static event Action OnRoundEnded;

        public static event Action OnRoundResultDisplayed;

        public static event Action OnRoundDataDisplayed;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SpawnLocalPlayerSpellManager();
            Phase.OnValueChanged += OnPhaseChanged;
        }

        public override void OnNetworkDespawn()
        {
            Phase.OnValueChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged(MofaPhase oldValue, MofaPhase newValue)
        {
            Debug.Log($"[MOFABase] Phase changed to {newValue}");
            switch (newValue)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    OnCoutdownStarted?.Invoke();
                    break;
                case MofaPhase.Fighting:
                    OnRoundStarted?.Invoke();
                    break;
                case MofaPhase.RoundOver:
                    OnRoundEnded?.Invoke();
                    break;
                case MofaPhase.RoundResult:
                    OnRoundResultDisplayed?.Invoke();
                    break;
                case MofaPhase.RoundData:
                    OnRoundDataDisplayed?.Invoke();
                    break;
            }
        }

        private void SpawnLocalPlayerSpellManager()
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
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.Fighting;
            yield return new WaitForSeconds(80f);
            Phase.Value = MofaPhase.RoundOver;
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.RoundResult;
            yield return new WaitForSeconds(3f);
            Phase.Value = MofaPhase.RoundData;
        }

        [ServerRpc]
        public void SpawnSpellServerRpc(int spellId, Vector3 clientCenterEyePosition,
            Quaternion clientCenterEyeRotation, ServerRpcParams serverRpcParams = default)
        {
            Spell spell = LocalPlayerSpellManager.SpellList.List[spellId];
            var position = clientCenterEyePosition + clientCenterEyeRotation * spell.SpawnOffset;
            var rotation = clientCenterEyeRotation;
            if (spell.PerpendicularToGround)
            {
                rotation = MofaUtils.GetHorizontalRotation(rotation);
            }
            var spellInstance = Instantiate(spell, position, rotation);
            spellInstance.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        }
    }
}