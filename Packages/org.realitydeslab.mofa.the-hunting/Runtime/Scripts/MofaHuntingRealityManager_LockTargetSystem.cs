// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;
using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheHunting
{
    public partial class MofaHuntingRealityManager
    {
        [Header("Lock Target System")]
        [SerializeField] private LockTargetPlayerIndicator _lockTargetPlayerIndicatorPrefab;

        private bool _isChoosingTarget = false;

        /// <summary>
        /// Keep a list of LockTargetPlayerIndicator references so that we can delete
        /// them in the future.
        /// </summary>
        private List<LockTargetPlayerIndicator> _playerIndicatorList = new();

        public static event Action OnTargetLocked;

        private void LockTargetSystem_Init()
        {
            UI.MofaHuntingDragonControllerUIPanel.OnDragonLockButtonPressed += OnDragonLockButtonPressed;
            UI.MofaHuntingDragonControllerUIPanel.OnDragonCancelLockButtonPressed += OnLockTargetSessionEnded;
        }

        private void LockTargetSystem_Deinit()
        {
            UI.MofaHuntingDragonControllerUIPanel.OnDragonLockButtonPressed -= OnDragonLockButtonPressed;
            UI.MofaHuntingDragonControllerUIPanel.OnDragonCancelLockButtonPressed -= OnLockTargetSessionEnded;
        }

        private void OnDragonLockButtonPressed()
        {
            if (_isChoosingTarget)
                return;

            _isChoosingTarget = true;
            //HoloKitApp.Instance.MultiplayerManager.ShowPoseVisualizers = true;
            foreach (var mofaPlayer in MofaPlayerList)
            {
                if (mofaPlayer.OwnerClientId == 0)
                    continue;

                var playerIndicatorInstance = Instantiate(_lockTargetPlayerIndicatorPrefab);
                playerIndicatorInstance.MofaPlayer = mofaPlayer;
                _playerIndicatorList.Add(playerIndicatorInstance);
            }
        }

        private void OnLockTargetSessionEnded()
        {
            if (!_isChoosingTarget)
                return;

            _isChoosingTarget = false;
            foreach (var playerIndicator in _playerIndicatorList)
            {
                Destroy(playerIndicator.gameObject);
            }
            _playerIndicatorList.Clear();
        }

        private void LockTargetSystem_Update()
        {
            if (_isChoosingTarget)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Raycast any LockTargetPlayerIndicator
                        Ray ray = HoloKitCameraManager.Instance.GetComponent<Camera>().ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out var hit))
                        {
                            if (hit.transform.CompareTag("Lock Target Player Indicator"))
                            {
                                Debug.Log("Target locked");
                                var playerIndicator = hit.transform.GetComponentInParent<LockTargetPlayerIndicator>();
                                var targetPlayer = playerIndicator.MofaPlayer;
                                OnTargetSelectedClientRpc(targetPlayer.OwnerClientId);
                                OnLockTargetSessionEnded();
                                OnTargetLocked?.Invoke();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When a new target is selected by the host, this selection operation should be
        /// replicated across the network.
        /// </summary>
        /// <param name="clientId">The client id of the target player</param>
        [ClientRpc]
        private void OnTargetSelectedClientRpc(ulong clientId)
        {
            var mofaHuntingRealityManager = HoloKitApp.Instance.RealityManager as MofaHuntingRealityManager;
            var mofaPlayerDict = mofaHuntingRealityManager.MofaPlayerDict;
            if (mofaHuntingRealityManager.MofaPlayerDict.ContainsKey(clientId))
            {
                var targetPlayer = mofaHuntingRealityManager.MofaPlayerDict[clientId];

                var targetLifeShield = targetPlayer.LifeShield;
                _dragonController.SetTarget(targetLifeShield.transform);
            }
            else
            {
                Debug.Log($"[LockTargetSystem] The new target player {clientId} is not found on the local device");
            }
        }
    }
}
