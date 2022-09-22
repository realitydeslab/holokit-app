using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    public class ShieldSpellVFXController : MonoBehaviour
    {
        [SerializeField] ShieldSpell _shieldController;
        [SerializeField] SpellLifetimeController _lifeTimeController;
        [SerializeField] Animator _animator;

        private void OnEnable()
        {
            if (_shieldController == null) Debug.LogError("not found attackSpell");
            _shieldController.OnBeingHit += OnBeingHit;
            _shieldController.OnBeingDestroyed += OnBeingDestoryed;

            _lifeTimeController.OnSpawned += OnSpawn;
            _lifeTimeController.OnDead += OnDie;
        }

        private void OnDisable()
        {
            _shieldController.OnBeingHit -= OnBeingHit;
            _shieldController.OnBeingDestroyed -= OnBeingDestoryed;

            _lifeTimeController.OnSpawned -= OnSpawn;
            _lifeTimeController.OnDead -= OnDie;

        }

        void OnSpawn()
        {

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
