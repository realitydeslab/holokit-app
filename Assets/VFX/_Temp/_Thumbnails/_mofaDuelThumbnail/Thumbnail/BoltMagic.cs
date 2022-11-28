using UnityEngine;
using Holoi.Library.HoloKitApp;

public class BoltMagic : MonoBehaviour
{
    private MOFATrainingThumbnail _pool;

    public void SetPool(MOFATrainingThumbnail pool)
    {
        _pool = pool;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetTrigger("Hit");
        StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
        {
            _pool.ReturnObjectToQueue(gameObject);
        }));
    }
}
