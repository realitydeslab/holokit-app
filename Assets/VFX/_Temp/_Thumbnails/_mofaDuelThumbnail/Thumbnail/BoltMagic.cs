using UnityEngine;
using Holoi.Library.HoloKitApp;

public class BoltMagic : MonoBehaviour
{
    private MOFATrainingThumbnail _pool;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetPool(MOFATrainingThumbnail pool)
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
