// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using RealityDesignLab.MOFA.Library.Base;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class DragonColliderController : MonoBehaviour, IDamageable
    {
        [SerializeField] private DragonController _theDragonController;

        [SerializeField] private int _multiplier = 1;

        private void Start()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        public void OnDamaged(ulong attackerClientId)
        {
            _theDragonController.OnDragonBeingHitClientRpc(transform.position);
            _theDragonController.OnDamaged(_multiplier, attackerClientId);
        }
    }
}
