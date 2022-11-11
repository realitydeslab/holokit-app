using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.MOFABase
{
    public class AvatarPlacementIndicatorController : MonoBehaviour
    {
        [SerializeField] Animator _animator;

        [SerializeField] VisualEffect _placementVFX;

        [SerializeField] VisualEffect _birthVFX;

        private void Start()
        {
            _placementVFX.enabled = true;
            _birthVFX.enabled = false;
        }

        //public void OnAppear()
        //{
        //    _placementVFX.enabled = true;
        //    _birthVFX.enabled = false;
        //}

        //public void OnDisappear()
        //{
        //    _placementVFX.enabled = false;
        //}

        public void OnBirth()
        {
            _birthVFX.enabled = true;
            _animator.SetTrigger("Birth");
        }
    }
}