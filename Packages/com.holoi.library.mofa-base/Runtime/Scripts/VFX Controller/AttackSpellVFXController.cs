using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class AttackSpellVFXController : MonoBehaviour
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
            
        }

        private void OnDestroy()
        {
            _attackSpell.OnHit -= OnHit;
            _lifetimeController.OnLifetimeEnded -= OnLifetimeEnded;
        }

        private void OnHit()
        {
            _animator.SetTrigger("Hit");
        }

        private void OnLifetimeEnded()
        {
            _animator.SetTrigger("Die");
        }
    }
}
