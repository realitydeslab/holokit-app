using System;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class LocalPlayerSpellManager : MonoBehaviour
    {
        public MagicSchoolList MagicSchoolList;

        public SpellList SpellList;

        [HideInInspector] public Spell BasicSpell;

        [HideInInspector] public Spell SecondarySpell;

        [HideInInspector] public bool Active;

        [HideInInspector] public float BasicSpellCharge;

        [HideInInspector] public float SecondarySpellCharge;

        [HideInInspector] public float SecondarySpellUseCount;

        [HideInInspector] public MofaWatchState CurrentWatchState;

        private MofaBaseRealityManager _mofaRealityManager;

        public float BasicSpellChargePercentage
        {
            get
            {
                return BasicSpellChargePercentage / BasicSpell.ChargeTime * BasicSpell.MaxChargeCount;
            }
        }

        public float SecondarySpellChargePercentage
        {
            get
            {
                return SecondarySpellCharge / SecondarySpell.ChargeTime;
            }
        }

        public static event Action<SpellType> OnSpawnSpellFailed;

        private void Awake()
        {
            SetupSpells();
            _mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
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
            foreach (var spell in SpellList.List)
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
                    break;
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
            }

            if (CurrentWatchState == MofaWatchState.Ground)
            {
                if (SecondarySpellCharge < SecondarySpell.ChargeTime)
                {
                    SecondarySpellCharge += Time.fixedDeltaTime;
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
                Debug.Log("[LocalPlayerSpellManager] Not active");
                return;
            }

            if (BasicSpellCharge < BasicSpell.ChargeTime)
            {
                Debug.Log("[LocalPlayerSpellManager] Basic spell not charged");
                OnSpawnSpellFailed?.Invoke(SpellType.Basic);
            }
            else
            {
                _mofaRealityManager.SpawnSpellServerRpc(BasicSpell.Id,
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
                Debug.Log("[LocalPlayerSpellManager] Not active");
                return;
            }

            if (SecondarySpellUseCount > SecondarySpell.MaxUseCount)
            {
                Debug.Log("[LocalPlayerSpellManager] Exceed secondary spell use count");
                OnSpawnSpellFailed?.Invoke(SpellType.Secondary);
                return;
            }

            _mofaRealityManager.SpawnSpellServerRpc(SecondarySpell.Id,
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
                if (_mofaRealityManager.Phase.Value == MofaPhase.Fighting)
                {
                    Active = true;
                    MofaWatchConnectivityAPI.QueryWatchState();
                }
            }
        }

        private void OnLifeShieldDestroyed(ulong ownerClientId)
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
            _mofaRealityManager.StartRound();
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