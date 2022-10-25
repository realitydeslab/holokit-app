using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.MOFABase
{
    public class AvatarPlacementIndicatorController : MonoBehaviour
    {
        //[SerializeField] Animator _animator;
        //[SerializeField] VisualEffect _placementVFX;
        //[SerializeField] VisualEffect _birthVFX;

        //void Start()
        //{
        //    _placementVFX.enabled = true;
        //    _birthVFX.enabled = false;

        //}

        public void OnBirth()
        {
            //_animator.SetTrigger("Birth");
            //_birthVFX.enabled = true;
        }
    }
}