using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.ARUX
{
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class LoadButtonController : MonoBehaviour
    {
        private Animator _animator;

        private MeshRenderer _meshRenderer;

        private HoverableObject _hoverableObject;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _hoverableObject = GetComponent<HoverableObject>();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _meshRenderer.material.SetFloat("_Load", _hoverableObject.CurrentLoadPercentage);
        }

        public void OnBirth()
        {
            gameObject.SetActive(true);
            _animator.Rebind();
            _animator.Update(0);
        }

        public void OnDisappear()
        {
            _animator.SetTrigger("Die");
        }

        public void OnDeath()
        {
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                Destroy(gameObject);
            }));
        }
    }
}