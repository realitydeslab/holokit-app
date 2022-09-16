using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Mofa.Base
{
    public enum ControllerState
    {
        Nothing = 0,
        Sky = 1,
        Ground = 2
    }

    public class LocalPlayerSpellManager : MonoBehaviour
    {
        public SpellList SpellList;

        [HideInInspector] public Spell BasicSpell;

        [HideInInspector] public Spell SecondarySpell;

        [HideInInspector] public bool Active;

        [HideInInspector] public float BasicSpellCharge;

        [HideInInspector] public float SecondarySpellCharge;

        [HideInInspector] public float SecondarySpellUseCount;

        [HideInInspector] public ControllerState ControllerState;

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

        public static event Action<ControllerState> OnControllerStateChanged;

        public static event Action<SpellType> OnSpawnSpellFailed;

        private void Awake()
        {
            SetupSpells();
            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        private void SetupSpells()
        {
            foreach (var spell in SpellList.List)
            {
                if (spell.MagicSchool.Id == 0)
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

            if (ControllerState == ControllerState.Ground)
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

        public void SpawnBasicSpell()
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
                    HoloKitCamera.Instance.CenterEyePose.position, HoloKitCamera.Instance.CenterEyePose.rotation);
                BasicSpellCharge -= BasicSpell.ChargeTime;
            }
        }

        public void SpawnSecondarySpell()
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

            if (SecondarySpellCharge < SecondarySpell.ChargeTime && HoloKitHelper.IsRuntime)
            {
                Debug.Log("[LocalPlayerSpellManager] Secondary spell not charged");
                OnSpawnSpellFailed?.Invoke(SpellType.Secondary);
            }
            else
            {
                _mofaRealityManager.SpawnSpellServerRpc(SecondarySpell.Id,
                    HoloKitCamera.Instance.CenterEyePose.position, HoloKitCamera.Instance.CenterEyePose.rotation);
                SecondarySpellCharge -= SecondarySpell.ChargeTime;
                SecondarySpellUseCount++;
            }
        }
    }
}