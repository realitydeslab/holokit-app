// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp.WatchConnectivity.MOFA;

namespace Holoi.Library.MOFABase
{
    public partial class MofaInputManager : MonoBehaviour
    {
        public float BasicSpellChargePercentage
        {
            get
            {
                if (_basicSpell == null)
                    return 0f;
                else
                    return _basicSpellCharge / (_basicSpell.ChargeTime * _basicSpell.MaxChargeCount);
            }
        }

        public float SecondarySpellChargePercentage
        {
            get
            {
                if (_secondarySpell == null)
                    return 0f;
                else
                    return _secondarySpellCharge / _secondarySpell.ChargeTime;
            }
        }

        public int SecondarySpellMaxUseCount
        {
            get
            {
                if (_secondarySpell == null)
                    return 0;
                else
                    return _secondarySpell.MaxUseCount;
            }
        }

        public int SecondarySpellUseCount
        {
            get
            {
                if (_secondarySpell == null)
                    return 0;
                else
                    return _secondarySpellUseCount;
            }
        }

        private bool _isActive;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        private float _basicSpellCharge;

        private float _secondarySpellCharge;

        private int _secondarySpellUseCount;

        private MofaBaseRealityManager _mofaBaseRealityManager;

        /// <summary>
        /// This event is called when the player tries to cast the basic spell
        /// but it is not charged.
        /// </summary>
        public static event Action OnBasicSpellNotCharged;

        /// <summary>
        /// This event is called when the player tries to cast the secondary spell
        /// but it has exceeded its max usage count.
        /// </summary>
        public static event Action OnSecondarySpellExceededMaxUseCount;

        private void Start()
        {
            _mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            InitializeWatchConnectivity();
            InitializeStarUI();
            SetupSpells();
        }

        private void OnDestroy()
        {
            DeinitializeWatchConnectivity();
            DeinitializeStarUI();
        }

        private void SetupSpells()
        {
            var preferencedMagicSchool = HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject();
            int count = 0;
            foreach (var spell in _mofaBaseRealityManager.SpellList.List)
            {
                if (spell.MagicSchool.TokenId.Equals(preferencedMagicSchool.TokenId))
                {
                    if (spell.SpellType == SpellType.Basic)
                    {
                        _basicSpell = spell;
                        count++;
                    }
                    else
                    {
                        _secondarySpell = spell;
                        count++;
                    }
                }

                if (count == 2)
                    break;
            }
            // Error check
            if (_basicSpell == null)
                Debug.LogError("[MofaInputManager] Failed to setup basic spell");
            if (_secondarySpell == null)
                Debug.LogError("[MofaInputManager] Failed to setup secondary spell");
        }

        /// <summary>
        /// This function is used to check whether the input manager is active
        /// at the current moment.
        /// </summary>
        /// <returns></returns>
        private void CheckIsActive()
        {
            if (_mofaBaseRealityManager.CurrentPhase.Value != MofaPhase.Fighting)
            {
                _isActive = false;
                return;
            }

            var localMofaPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
            if (localMofaPlayer == null)
            {
                _isActive = false;
                return;
            }

            if (localMofaPlayer.LifeShield == null)
            {
                _isActive = false;
                return;
            }

            if (localMofaPlayer.LifeShield.IsDestroyed)
            {
                _isActive = false;
                return;
            }

            _isActive = true;
            return;
        }

        private void FixedUpdate()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
                return;

            CheckIsActive();
            if (!_isActive)
                return;

            if (_basicSpellCharge < _basicSpell.ChargeTime * _basicSpell.MaxChargeCount)
            {
                _basicSpellCharge += Time.fixedDeltaTime;
                if (_basicSpellCharge > _basicSpell.ChargeTime * _basicSpell.MaxChargeCount)
                {
                    _basicSpellCharge = _basicSpell.ChargeTime * _basicSpell.MaxChargeCount;
                }
            }

            if (CurrentWatchState == MofaWatchState.Ground)
            {
                if (_secondarySpellCharge < _secondarySpell.ChargeTime)
                {
                    _secondarySpellCharge += Time.fixedDeltaTime;
                    if (_secondarySpellCharge > _secondarySpell.ChargeTime)
                    {
                        _secondarySpellCharge = _secondarySpell.ChargeTime;
                    }
                }
            }
            else
            {
                if (_secondarySpellCharge > 0f)
                {
                    _secondarySpellCharge -= Time.fixedDeltaTime;
                }
            }
        }

        public void Reset()
        {
            _basicSpellCharge = 0f;
            _secondarySpellCharge = 0f;
            _secondarySpellUseCount = 0;
        }

        private void TryCastSpell()
        {
            if (_secondarySpellCharge >= _secondarySpell.ChargeTime)
                TryCastSecondarySpell();
            else
                TryCastBasicSpell();
        }

        private void TryCastBasicSpell()
        {
            if (!_isActive)
            {
                Debug.Log("[MofaInputManager] Cannot cast spell when inactive");
                return;
            }

            if (_basicSpellCharge < _basicSpell.ChargeTime)
            {
                //Debug.Log("[MofaInputManager] Basic spell not charged");
                OnBasicSpellNotCharged?.Invoke();
                return;
            }

            CastBasicSpell();
        }

        private void CastBasicSpell()
        {
            _mofaBaseRealityManager.SpawnSpellServerRpc(_basicSpell.Id,
                HoloKitCameraManager.Instance.CenterEyePose.position,
                GetCameraGravitationalRotation(),
                NetworkManager.Singleton.LocalClientId);
            _basicSpellCharge -= _basicSpell.ChargeTime;
        }

        private void TryCastSecondarySpell()
        {
            if (!_isActive)
            {
                Debug.Log("[MofaInputManager] Cannot cast spell when inactive");
                return;
            } 

            if (_secondarySpellUseCount > _secondarySpell.MaxUseCount)
            {
                Debug.Log("[MofaInputManager] Secondary spell exceeded max use count");
                OnSecondarySpellExceededMaxUseCount?.Invoke();
                return;
            }

            CastSecondarySpell();
        }

        private void CastSecondarySpell()
        {
            _mofaBaseRealityManager.SpawnSpellServerRpc(_secondarySpell.Id,
                HoloKitCameraManager.Instance.CenterEyePose.position,
                GetCameraGravitationalRotation(),
                NetworkManager.Singleton.LocalClientId);
            _secondarySpellCharge = 0f;
            _secondarySpellUseCount++;
        }

        private Quaternion GetCameraGravitationalRotation()
        {
            Vector3 cameraRotationEuler = HoloKitCameraManager.Instance.CenterEyePose.rotation.eulerAngles;
            return Quaternion.Euler(cameraRotationEuler.x, cameraRotationEuler.y, 0f);
        }
    }
}