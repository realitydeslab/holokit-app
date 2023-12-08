// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhaGroup : NetworkBehaviour
    {
        [SerializeField] private List<BuddhaController> _buddhas = new();

        private NetworkVariable<int> _currentBuddhaIndex = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            UI.QuantumRealmUIPanel.OnSwitchButtonPressed += OnSwitchButtonPressed;
            ARObjectAdjuster.Instance.SetARObject(transform);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((QuantumRealmRealityManager)HoloKitApp.Instance.RealityManager).SetBuddhaGroup(this);
            _currentBuddhaIndex.OnValueChanged += OnCurrentBuddhaIndexChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _currentBuddhaIndex.OnValueChanged -= OnCurrentBuddhaIndexChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UI.QuantumRealmUIPanel.OnSwitchButtonPressed -= OnSwitchButtonPressed;
        }

        private void OnCurrentBuddhaIndexChanged(int oldValue, int newValue)
        {
            if (oldValue + 1 == newValue || (oldValue == _buddhas.Count - 1 && newValue == 0))
            {
                Debug.Log($"[BuddhaGroup] Activated budda with index {newValue}");
                _buddhas[oldValue].OnDeactivated();
                _buddhas[newValue].OnActivated();
            }
        }

        public void ActivateNextBuddha()
        {
            if (!HoloKitApp.Instance.IsHost) { return; }

            if (_currentBuddhaIndex.Value == _buddhas.Count - 1)
            {
                _currentBuddhaIndex.Value = 0;
            }
            else
            {
                _currentBuddhaIndex.Value++;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ActivateNextBuddhaServerRpc()
        {
            ActivateNextBuddha();
        }

        private void OnSwitchButtonPressed()
        {
            ActivateNextBuddhaServerRpc();
        }
    }
}