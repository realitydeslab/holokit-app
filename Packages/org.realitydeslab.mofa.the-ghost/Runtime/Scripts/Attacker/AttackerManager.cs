// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.MOFA.Library.Base;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.TheGhost
{
    public partial class AttackerManager : NetworkBehaviour
    {
        [SerializeField] private SpellList _spellList;

        private void Awake()
        {
            AddSpellPrefabsToNetworkPrefabList();
        }

        private void Start()
        {
            // Only client players will cast spells
            if (HoloKitApp.Instance.IsPlayer && HoloKitApp.Instance.LocalPlayerTypeSubindex == 1)
            {
                Init_WatchConnectivity();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (HoloKitApp.Instance.IsPlayer && HoloKitApp.Instance.LocalPlayerTypeSubindex == 1)
            {
                Deinit_WatchConnectivity();
            }
        }

        private void AddSpellPrefabsToNetworkPrefabList()
        {
            // Get the reference of the RealityConfiguration component
            var realityConfig = FindObjectOfType<RealityConfiguration>();

            // Instead of manually dragged every prefab onto the list, we use a
            // for loop to add them
            foreach (var spell in _spellList.List)
            {
                if (!realityConfig.NetworkPrefabs.Contains(spell.gameObject))
                {
                    realityConfig.NetworkPrefabs.Add(spell.gameObject);
                }
            }
        }

        /// <summary>
        /// Spawn a spell across the network.
        /// </summary>
        /// <param name="id">The spell id</param>
        /// <param name="centerEyePosition">The center eye position of the spell owner</param>
        /// <param name="rotation">The center eye rotation of the spell owner</param>
        /// <param name="serverRpcParams">This is used to send the owner client id</param>
        [ServerRpc(RequireOwnership = false)]
        private void SpawnSpellServerRpc(int id, Vector3 centerEyePosition, Quaternion centerEyeRotation, ServerRpcParams serverRpcParams = default)
        {
            // Get the spell prefab we are going to spawn
            var spell = _spellList.GetSpell(id);
            // Transform the spell offset from local space to world space
            Vector3 spellPosition = centerEyePosition + centerEyeRotation * spell.SpawnOffset;
            Quaternion spellRotation = centerEyeRotation;

            // Spawn the spell locally
            var spellInstance = Instantiate(spell, spellPosition, spellRotation);
            // Spawn the spell across the network with owner client id
            spellInstance.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        }
    }
}
