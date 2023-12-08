// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class RedScreenController : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;

        private void Start()
        {
            LifeShield.OnBeingHit += OnLifeShieldBeingHit;
        }

        private void OnDestroy()
        {
            LifeShield.OnBeingHit -= OnLifeShieldBeingHit;
        }

        public void OnLifeShieldBeingHit(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                vfx.SendEvent("OnBeingHit");
            }
        }
    }
}
