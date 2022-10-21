using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class AttackSpellVFXController : MonoBehaviour
    {
        [SerializeField] AttackSpell _attackController;
        [SerializeField] SpellLifetimeController _lifeTimeController;
        [SerializeField] Animator _animator;

        private void OnEnable()
        {
            if (_attackController == null) Debug.LogError("not found attackSpell");
            _attackController.OnHit += OnHit;
            _lifeTimeController.OnSpawned += OnSpawn;
            _lifeTimeController.OnDead += OnDie;
        }

        private void OnDisable()
        {
            _attackController.OnHit -= OnHit;
            _lifeTimeController.OnSpawned -= OnSpawn;
            _lifeTimeController.OnDead -= OnDie;
        }

        void OnSpawn()
        {

        }
        void OnHit()
        {
            _animator.SetTrigger("Hit");
        }

        void OnDie()
        {
            _animator.SetTrigger("Die");
        }
    }
}
