using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.MOFATheDucks
{
    public class ThumbnailDuckMagicController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _duckVfx;
        [SerializeField] private Animator _animator;

        private void OnBeingHit()
        {
            _animator.SetTrigger("Hit");
            _duckVfx.SendEvent("OnHit");
            _duckVfx.SetBool("Duck Alive", false);
        }
    }
}
