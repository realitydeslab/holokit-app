using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class MysticArtAttackSpellVFXController : MonoBehaviour
    {
        [SerializeField] private AttackSpell _attackSpell;

        [SerializeField] private SpellLifetimeController _lifetimeController;

        [SerializeField] private Animator _animator;

        private void Start()
        {
            _attackSpell.OnHit += OnHit;
            _lifetimeController.OnLifetimeEnded += OnLifetimeEnded;
        }

        private void OnDisable()
        {
            // Reset
            _animator.Rebind();
            _animator.Update(0f);
        }

        private void OnDestroy()
        {
            _attackSpell.OnHit -= OnHit;
            _lifetimeController.OnLifetimeEnded -= OnLifetimeEnded;
        }

        public void OnHit()
        {
            if (_attackSpell.HitOnce)
            {
                _animator.SetTrigger("Hit");
            }
        }

        public void OnLifetimeEnded()
        {
            _animator.SetTrigger("Die");
        }
    }
}
