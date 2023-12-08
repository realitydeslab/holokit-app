// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class ShieldSpellVFXController : MonoBehaviour
    {
        [SerializeField] ShieldSpell _shieldController;
        [SerializeField] SpellLifetimeController _lifeTimeController;
        [SerializeField] Animator _animator;

        private void Start()
        {
            _shieldController.OnBeingHit += OnBeingHit;
            _shieldController.OnBeingDestroyed += OnBeingDestoryed;
            _lifeTimeController.OnLifetimeEnded += OnDie;
        }

        private void OnDisable()
        {
            // Reset
            _animator.Rebind();
            _animator.Update(0f);
        }

        private void OnDestroy()
        {
            _shieldController.OnBeingHit -= OnBeingHit;
            _shieldController.OnBeingDestroyed -= OnBeingDestoryed;
            _lifeTimeController.OnLifetimeEnded -= OnDie;
        }

        void OnBeingHit()
        {
            _animator.SetTrigger("BeingHit");
        }

        void OnBeingDestoryed()
        {
            _animator.SetTrigger("Die");
        }

        void OnDie()
        {
            _animator.SetTrigger("Die");
        }
    }
}
