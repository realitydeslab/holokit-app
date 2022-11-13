using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.MOFABase
{
    public class AvatarPlacementIndicatorController : MonoBehaviour
    {
        [HideInInspector] public Vector3 HitPosition;

        [SerializeField] Animator _animator;

        [SerializeField] VisualEffect _hookVFX;

        [SerializeField] VisualEffect _placementVFX;

        [SerializeField] VisualEffect _birthVFX;

        Transform _centerEye;

        private void Start()
        {
            _centerEye = HoloKit.HoloKitCamera.Instance.CenterEyePose;

            _hookVFX.enabled = true;

            _placementVFX.enabled = true;

            _birthVFX.enabled = false;
        }

        private void Update()
        {
            var pos = new Vector3(_centerEye.position.x, HitPosition.y, _centerEye.position.z);

            transform.position = pos;

            _placementVFX.transform.position = HitPosition;

            _birthVFX.transform.position = HitPosition;

            _hookVFX.SetVector3("Hit Position", HitPosition);
        }

        public void OnBirth()
        {
            _birthVFX.enabled = true;

            _animator.SetTrigger("Birth");
        }
    }
}