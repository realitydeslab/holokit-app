using System;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.HoloKitApp.WatchConnectivity;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class MofaInputManager : MonoBehaviour
    {
        [HideInInspector] public Spell BasicSpell;

        [HideInInspector] public Spell SecondarySpell;

        [HideInInspector] public bool Active;

        [HideInInspector] public float BasicSpellCharge;

        [HideInInspector] public float SecondarySpellCharge;

        [HideInInspector] public float SecondarySpellUseCount;

        [HideInInspector] public MofaWatchState CurrentWatchState;

        public float BasicSpellChargePercentage
        {
            get
            {
                return BasicSpellCharge / (BasicSpell.ChargeTime * BasicSpell.MaxChargeCount);
            }
        }

        public float SecondarySpellChargePercentage
        {
            get
            {
                if (SecondarySpell != null)
                {
                    return SecondarySpellCharge / SecondarySpell.ChargeTime;
                }
                else
                {
                    return 0f;
                }
            }
        }

        private MofaBaseRealityManager MofaBaseRealityManager => (MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager;

        public static event Action<SpellType> OnSpawnSpellFailed;

        private void Start()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
            {
                Destroy(gameObject);
                return;
            }

            // Setup MofaWatchConnectivity
            MofaWatchConnectivityAPI.Initialize();
            // MofaWatchConnectivityManager should take control first
            MofaWatchConnectivityAPI.TakeControlWatchConnectivitySession();
            // We then update the control on Watch side so that MofaWatchConnectivityManager won't miss messages.
            HoloKitAppWatchConnectivityAPI.UpdateCurrentReality(WatchReality.MOFATheTraining);

            SetupSpells();
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            LifeShield.OnSpawned += OnLifeShieldSpawned;
            LifeShield.OnDead += OnLifeShieldDestroyed;

            HoloKitAppUIEventManager.OnTriggered += OnStarUITriggered;
            HoloKitAppUIEventManager.OnBoosted += OnStarUIBoosted;

            MofaWatchConnectivityAPI.OnStartRoundMessageReceived += OnStartRoundMessageReceived;
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnWatchTriggered;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            LifeShield.OnSpawned -= OnLifeShieldSpawned;
            LifeShield.OnDead -= OnLifeShieldDestroyed;

            HoloKitAppUIEventManager.OnTriggered -= OnStarUITriggered;
            HoloKitAppUIEventManager.OnBoosted -= OnStarUIBoosted;

            MofaWatchConnectivityAPI.OnStartRoundMessageReceived -= OnStartRoundMessageReceived;
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered -= OnWatchTriggered;
        }

        private void SetupSpells()
        {
            var preferencedMofaSpell = HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject(null);
            
            foreach (var spell in MofaBaseRealityManager.SpellList.List)
            {
                if (spell.MagicSchool.TokenId.Equals(preferencedMofaSpell.TokenId))
                {
                    if (spell.SpellType == SpellType.Basic)
                    {
                        BasicSpell = spell;
                    }
                    else
                    {
                        SecondarySpell = spell;
                    }
                }
            }
        }

        private void OnPhaseChanged(MofaPhase newPhase)
        {
            switch (newPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    Reset();
                    MofaWatchConnectivityAPI.SyncRoundStartToWatch();
                    break;
                case MofaPhase.Fighting:
                    Active = true;
                    MofaWatchConnectivityAPI.QueryWatchState();
                    break;
                case MofaPhase.RoundOver:
                    Active = false;
                    break;
                case MofaPhase.RoundResult:
                    break;
                case MofaPhase.RoundData:
                    OnRoundData();
                    break;
            }
        }

        private void OnRoundData()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                
                var localPlayerStats = ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).GetIndividualStats();
                MofaWatchConnectivityAPI.SyncRoundResultToWatch(localPlayerStats.IndividualRoundResult,
                                                                localPlayerStats.Kill,
                                                                localPlayerStats.HitRate,
                                                                localPlayerStats.Distance);
            }
        }

        private void FixedUpdate()
        {
            if (!Active)
            {
                return;
            }

            if (BasicSpellCharge < BasicSpell.ChargeTime * BasicSpell.MaxChargeCount)
            {
                BasicSpellCharge += Time.fixedDeltaTime;
                if (BasicSpellCharge > BasicSpell.ChargeTime * BasicSpell.MaxChargeCount)
                {
                    BasicSpellCharge = BasicSpell.ChargeTime * BasicSpell.MaxChargeCount;
                }
            }

            if (CurrentWatchState == MofaWatchState.Ground)
            {
                if (SecondarySpellCharge < SecondarySpell.ChargeTime)
                {
                    SecondarySpellCharge += Time.fixedDeltaTime;
                    if (SecondarySpellCharge > SecondarySpell.ChargeTime)
                    {
                        SecondarySpellCharge = SecondarySpell.ChargeTime;
                    }
                }
            }
            else
            {
                if (SecondarySpellCharge > 0f)
                {
                    SecondarySpellCharge -= Time.fixedDeltaTime;
                }
            }
        }

        public void Reset()
        {
            BasicSpellCharge = 0f;
            SecondarySpellCharge = 0f;
            SecondarySpellUseCount = 0;
        }

        private void SpawnBasicSpell()
        {
            if (!Active)
            {
                Debug.Log("[MofaInputManager] Not active");
                return;
            }

            if (BasicSpellCharge < BasicSpell.ChargeTime)
            {
                Debug.Log("[MofaInputManager] Basic spell not charged");
                OnSpawnSpellFailed?.Invoke(SpellType.Basic);
            }
            else
            {
                MofaBaseRealityManager.SpawnSpellServerRpc(BasicSpell.Id,
                    HoloKitCamera.Instance.CenterEyePose.position,
                    HoloKitCamera.Instance.CenterEyePose.rotation,
                    NetworkManager.Singleton.LocalClientId);
                BasicSpellCharge -= BasicSpell.ChargeTime;
            }
        }

        private void SpawnSecondarySpell()
        {
            if (!Active)
            {
                Debug.Log("[MofaInputManager] Not active");
                return;
            }

            if (SecondarySpellUseCount > SecondarySpell.MaxUseCount)
            {
                Debug.Log("[MofaInputManager] Exceed secondary spell use count");
                OnSpawnSpellFailed?.Invoke(SpellType.Secondary);
                return;
            }

            MofaBaseRealityManager.SpawnSpellServerRpc(SecondarySpell.Id,
                HoloKitCamera.Instance.CenterEyePose.position,
                HoloKitCamera.Instance.CenterEyePose.rotation,
                NetworkManager.Singleton.LocalClientId);
            SecondarySpellCharge -= SecondarySpell.ChargeTime;
            SecondarySpellUseCount++;
        }

        private void OnLifeShieldSpawned(ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                if (MofaBaseRealityManager.CurrentPhase == MofaPhase.Fighting)
                {
                    Active = true;
                    MofaWatchConnectivityAPI.QueryWatchState();
                }
            }
        }

        private void OnLifeShieldDestroyed(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                Active = false;
            }
        }

        private void OnStarUITriggered()
        {
            if (Active)
            {
                SpawnBasicSpell();
            }
        }

        private void OnStarUIBoosted()
        {
            if (Active)
            {
                SpawnSecondarySpell();
            }
        }

        #region Apple Watch
        private void OnStartRoundMessageReceived()
        {
            ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).GetPlayer().Ready.Value = true;
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            CurrentWatchState = watchState;
        }

        private void OnWatchTriggered()
        {
            if (Active)
            {
                if (SecondarySpellCharge >= SecondarySpell.ChargeTime)
                {
                    SpawnSecondarySpell();
                }
                else
                {
                    SpawnBasicSpell();
                }
            }
        }
        #endregion
    }
}