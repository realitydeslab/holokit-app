using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOFATrainingThumbnail : MonoBehaviour
{
    [SerializeField] Animator _avatarAnimator;
    [SerializeField] Animator _boltAnimator;
    [SerializeField] Transform _boltMagic;

    void Start()
    {
        StartCoroutine(WaitAndShoot());
    }

    void Update()
    {
        
    }

    IEnumerator WaitAndShoot()
    {
        yield return new WaitForSecondsRealtime(3f);
        _avatarAnimator.SetTrigger("Attack A");
        yield return new WaitForSecondsRealtime(0.25f);
        ShootBolt();
        StartCoroutine(WaitAndShoot());
    }

    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        _boltAnimator.SetTrigger("Hit");
        _boltMagic.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    void ShootBolt()
    {
        _boltAnimator.enabled = true;
        _boltAnimator.Rebind();
        _boltMagic.transform.position = new Vector3(0, 1.5f, 0);
        _boltMagic.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -2f);
        StartCoroutine(WaitAndExplode());
    }
}
