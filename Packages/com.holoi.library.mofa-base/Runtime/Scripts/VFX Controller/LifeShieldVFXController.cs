// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class LifeShieldVFXController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _debrisExplosion; // 0 1 2 3 = center top left right

        private LifeShield _lifeShield;

        private void Start()
        {
            _lifeShield = GetComponentInParent<LifeShield>();
            _lifeShield.OnCenterDestroyed += OnCenterDestroyed;
            _lifeShield.OnTopDestroyed += OnTopDestroyed;
            _lifeShield.OnLeftDestroyed += OnLeftDestroyed;
            _lifeShield.OnRightDestroyed += OnRightDestroyed;
            LifeShield.OnRenovated += OnRenovated;
        }

        private void OnDestroy()
        {
            _lifeShield.OnCenterDestroyed -= OnCenterDestroyed;
            _lifeShield.OnTopDestroyed -= OnTopDestroyed;
            _lifeShield.OnLeftDestroyed -= OnLeftDestroyed;
            _lifeShield.OnRightDestroyed -= OnRightDestroyed;
            LifeShield.OnRenovated -= OnRenovated;
        }

        private void OnCenterDestroyed()
        {
            _debrisExplosion[0].SetActive(true);
        }

        private void OnTopDestroyed()
        {
            _debrisExplosion[1].SetActive(true);
        }

        private void OnLeftDestroyed()
        {
            _debrisExplosion[2].SetActive(true);
        }

        private void OnRightDestroyed()
        {
            _debrisExplosion[3].SetActive(true);
        }

        private void OnRenovated(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.GetComponent<NetworkObject>().OwnerClientId)
            {
                foreach (var debris in _debrisExplosion)
                {
                    debris.SetActive(false);
                }
            }
        }
    }
}
