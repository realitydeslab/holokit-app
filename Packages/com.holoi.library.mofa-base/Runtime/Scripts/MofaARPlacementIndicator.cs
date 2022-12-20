using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    public class MofaARPlacementIndicator : MonoBehaviour
    {
        [SerializeField] private Transform _hitPoint;

        [SerializeField] private Animator _animator;

        [SerializeField] private VisualEffect _hookVFX;

        [SerializeField] private VisualEffect _placementVFX;

        [SerializeField] private VisualEffect _birthVFX;

        [SerializeField] private float _destroyDelay = 2f;

        public SkinnedMeshRenderer _avatarRenderer;

        private bool _isActive;

        private void Start()
        {
            _isActive = true;
            _hookVFX.enabled = true;
            _placementVFX.enabled = true;
            _birthVFX.enabled = false;
        }

        private void Update()
        {
            if (_hitPoint == null || !_isActive) { return; }

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

        public void OnPlaced(bool destroy)
        {
            _isActive = false;
            _hookVFX.gameObject.SetActive(false);
            _placementVFX.gameObject.SetActive(false);

            //_birthVFX.SetSkinnedMeshRenderer("SkinnerMeshRenderer", _avatarRenderer);
            //_birthVFX.gameObject.SetActive(true);
            //_birthVFX.enabled = true;

            _animator.SetTrigger("Birth");
            if (destroy)
            {
                Destroy(gameObject, _destroyDelay);
            }
            else
            {
                StartCoroutine(HoloKitAppUtils.WaitAndDo(_destroyDelay, () =>
                {
                    gameObject.SetActive(false);
                }));
            }
        }

        public void OnDisabled(bool destroy)
        {
            _isActive = false;
            _hookVFX.gameObject.SetActive(false);
            _placementVFX.gameObject.SetActive(false);
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnRestart()
        {
            _isActive = true;
            gameObject.SetActive(true);
            _hookVFX.gameObject.SetActive(true);
            _placementVFX.gameObject.SetActive(true);
        }
    }
}