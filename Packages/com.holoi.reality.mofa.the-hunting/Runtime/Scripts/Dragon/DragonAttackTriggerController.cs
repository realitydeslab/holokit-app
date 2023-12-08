// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFA.TheHunting
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
