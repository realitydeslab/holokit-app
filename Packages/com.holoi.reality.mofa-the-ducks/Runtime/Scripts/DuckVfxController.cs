using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.MOFATheDucks
{
    public class DuckVfxController : MonoBehaviour
    {
        /// <summary>
        /// Keep a reference of the Duck script in order to access its events.
        /// </summary>
        [SerializeField] private Duck _duck;

        /// <summary>
        /// You may not use a VFX for the duck. Feel free to replace this with
        /// anything you want.
        /// </summary>
        [SerializeField] private VisualEffect _duckVfx;
        [SerializeField] private Animator _animator;

        private void Start()
        {
            _duck.OnNaturalDeath += OnNaturalDeath;
            _duck.OnBeingHit += OnBeingHit;
        }

        private void OnNaturalDeath()
        {
            _animator.SetTrigger("Death");
        }

        private void OnBeingHit()
        {
            _duckVfx.SendEvent("OnHit");
            _duckVfx.SetBool("Duck Alive", false);
        }

        void OnTriggerDeathPaticle()
        {
            _duckVfx.SendEvent("OnDeath");

        }
    }
}
