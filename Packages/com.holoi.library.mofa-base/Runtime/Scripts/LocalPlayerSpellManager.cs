using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Holoi.HoloKit.App;
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

        public static event Action<SpellType> OnSpawnSpellFailed;

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

        private void Awake()
        {
            // TODO: Setup two spells

            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
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
            if (SecondarySpellUseCount > SecondarySpell.MaxUseCount)
            {
                Debug.Log("[LocalPlayerSpellManager] Exceed secondary spell use count");
                OnSpawnSpellFailed?.Invoke(SpellType.Secondary);
                return;
            }

            if (SecondarySpellCharge < SecondarySpell.ChargeTime)
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