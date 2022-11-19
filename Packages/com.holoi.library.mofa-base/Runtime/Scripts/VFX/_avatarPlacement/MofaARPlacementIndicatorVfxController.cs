using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.MOFABase
{
    public class MofaARPlacementIndicatorVfxController : MonoBehaviour
    {
        [SerializeField] private Transform _hitPoint;

        [SerializeField] private Animator _animator;

        [SerializeField] private VisualEffect _hookVFX;

        [SerializeField] private VisualEffect _placementVFX;

        [SerializeField] private VisualEffect _birthVFX;

        private void Start()
        {
            _hookVFX.enabled = true;
            _placementVFX.enabled = true;
            _birthVFX.enabled = false;
        }

        private void Update()
        {
            if (_hitPoint == null) { return; }

            if (_hitPoint.gameObject.activeSelf)
            {
                _hookVFX.gameObject.SetActive(true);
                _placementVFX.gameObject.SetActive(true);

                var centerEyePose = HoloKit.HoloKitCamera.Instance.CenterEyePose;
                var pos = new Vector3(centerEyePose.position.x, _hitPoint.position.y, centerEyePose.position.z);
                var direction = (_hitPoint.position - pos).normalized;

                _hookVFX.transform.localPosition = pos;
                _hookVFX.transform.LookAt(pos + direction);
                _hookVFX.SetVector3("Hit Position", _hitPoint.position);
                _placementVFX.transform.localPosition = _hitPoint.position;
                _birthVFX.transform.localPosition = _hitPoint.position;
            }
            else
            {
                _hookVFX.gameObject.SetActive(false);
                _placementVFX.gameObject.SetActive(false);
            }
        }

        public void OnPlaced()
        {
            _hitPoint = null;
            _hookVFX.gameObject.SetActive(false);
            _birthVFX.enabled = true;
            _animator.SetTrigger("Birth");
            Destroy(gameObject, 2f);
        }

        public void OnDisabled()
        {
            _hitPoint = null;
            _hookVFX.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}