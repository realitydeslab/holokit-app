// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp;
using org.realitydeslab.MOFABase;

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
