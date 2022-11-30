using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    public class MofaThumbnailBoltMagic : MonoBehaviour
    {
        private MofaThumbnailAvatar _pool;

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetPool(MofaThumbnailAvatar pool)
        {
            _pool = pool;
        }

        private void OnTriggerEnter(Collider other)
        {
            _animator.SetTrigger("Hit");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
            // Reset animator
            _animator.Rebind();
                _animator.Update(0f);
                _pool.ReturnObjectToQueue(gameObject);
            }));
        }
    }
}
