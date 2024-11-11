// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    public class MofaFightingPanelStatus : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private VisualEffect _lifeCircles;

        [SerializeField] private VisualEffect _attackBar;

        [SerializeField] private VisualEffect _ultimateBar;

        private MofaInputManager _inputManager;

        // The life shield of the local player.
        private LifeShield _localPlayerLifeShield;

        private void Start()
        {
            _inputManager = FindAnyObjectByType<MofaInputManager>();
        }

        private void Update()
        {
            if (_inputManager.CurrentWatchState == MofaWatchState.Ground)
                _animator.SetBool("Attack", false);
            else
                _animator.SetBool("Attack", true);

            // Update spell charging
            _attackBar.SetFloat("Loading Process", _inputManager.BasicSpellChargePercentage);
            _ultimateBar.SetFloat("Loading Process", _inputManager.SecondarySpellChargePercentage);

            // Update life shield petals
            if (_localPlayerLifeShield)
            {
                _lifeCircles.SetBool("Center", !_localPlayerLifeShield.CenterDestroyed.Value);
                _lifeCircles.SetBool("Up", !_localPlayerLifeShield.TopDestroyed.Value);
                _lifeCircles.SetBool("Left", !_localPlayerLifeShield.LeftDestroyed.Value);
                _lifeCircles.SetBool("Right", !_localPlayerLifeShield.RightDestroyed.Value);
            }
            else
            {
                TryFindLocalPlayerLifeShield();
            }
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            _localPlayerLifeShield = lifeShield;
        }

        private void TryFindLocalPlayerLifeShield()
        {
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var localMofaPlayer = mofaBaseRealityManager.LocalMofaPlayer;
            if (localMofaPlayer == null)
                return;

            if (localMofaPlayer.LifeShield == null)
                return;

            _localPlayerLifeShield = localMofaPlayer.LifeShield;
        }
    }
}