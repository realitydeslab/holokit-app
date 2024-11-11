// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class PortalController : NetworkBehaviour
    {
        [SerializeField] private float _lifetime = 10;

        private void Start()
        {
            Destroy(gameObject, _lifetime);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetPortalController(this);
        }
    }
}
