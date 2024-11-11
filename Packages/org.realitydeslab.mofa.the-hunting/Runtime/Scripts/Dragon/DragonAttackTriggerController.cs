// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.Library.Base;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class DragonAttackTriggerController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (HoloKitApp.Instance.IsHost)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (other.GetComponentInParent<NetworkObject>().OwnerClientId != 0)
                    {
                        damageable.OnDamaged(0);
                    }
                }
            }
        }
    }
}
