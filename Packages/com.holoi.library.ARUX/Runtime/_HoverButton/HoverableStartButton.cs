using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.ARUX
{
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class HoverableStartButton : MonoBehaviour
    {
        private Animator _animator;

        private MeshRenderer _meshRenderer;

        private HoverableObject _hoverableObject;

        private void Update()
        {
            _meshRenderer.material.SetFloat("_Load", _hoverableObject.CurrentLoadPercentage);
        }

        public void OnAppear()
        {
            gameObject.SetActive(true);

            _animator = GetComponent<Animator>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _hoverableObject = GetComponent<HoverableObject>();

            _animator.Rebind();
            _animator.Update(0);
        }

        public void OnDisappear()
        {
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                gameObject.SetActive(false);
            }));
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