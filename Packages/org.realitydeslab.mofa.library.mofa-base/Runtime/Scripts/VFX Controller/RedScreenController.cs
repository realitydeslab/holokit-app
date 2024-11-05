// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace RealityDesignLab.MOFA.Library.MOFABase
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
