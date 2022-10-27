using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOFATrainingThumbnail : MonoBehaviour
{
    [SerializeField] Animator _avatarAnimator;
    [SerializeField] float _avatarVelocity;
    Animator _boltAnimator;
    [SerializeField] GameObject _boltPrefab;
    [SerializeField] float _attackInterval = 3f;
    [SerializeField] float _attackPreset = 0f;

    void Start()
    {
        if (_avatarAnimator == null)
            _avatarAnimator = GetComponent<Animator>();
        if (_avatarVelocity != 0) _avatarAnimator.SetFloat("Velocity X", _avatarVelocity);

        StartCoroutine(WaitAndBegin(_attackPreset, _attackInterval));
    }

    void Update()
    {

    }

    IEnumerator WaitAndBegin(float time, float interval)
    {
        yield return new WaitForSecondsRealtime(time);
        StartCoroutine(WaitAndShoot(interval));
    }
    IEnumerator WaitAndShoot(float time)
    {
        yield return new WaitForSecondsRealtime(time - 0.25f);
        _avatarAnimator.SetTrigger("Attack A");
        yield return new WaitForSecondsRealtime(0.25f);
        ShootBolt();
        StartCoroutine(WaitAndShoot(_attackInterval));
    }

    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        _boltAnimator.SetTrigger("Hit");
        _boltPrefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void ShootBolt()
    {
        var go = Instantiate(_boltPrefab);
        _boltAnimator = go.GetComponent<Animator>();
        go.transform.LookAt(transform.forward * 5f);
        go.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1f;
        go.GetComponent<Rigidbody>().velocity = transform.forward * 2f;
        //StartCoroutine(WaitAndExplode());
    }
}
