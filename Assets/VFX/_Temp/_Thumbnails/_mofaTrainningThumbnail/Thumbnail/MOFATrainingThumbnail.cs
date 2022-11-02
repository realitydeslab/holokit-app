using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOFATrainingThumbnail : MonoBehaviour
{
    [SerializeField] Transform _parent;
    [SerializeField] Animator _avatarAnimator;
    [SerializeField] Vector2 _avatarVelocity;
    Animator _boltAnimator;
    [SerializeField] GameObject _boltPrefab;
    [SerializeField] float _attackInterval = 3f;
    [SerializeField] float _attackPreset = 0f;

    void Start()
    {
        if (_avatarAnimator == null)
            _avatarAnimator = GetComponent<Animator>();
        if (_avatarVelocity.magnitude != 0)
        {
            _avatarAnimator.SetFloat("Velocity Z", _avatarVelocity.x);
            _avatarAnimator.SetFloat("Velocity X", _avatarVelocity.y);

        }

        if (_boltPrefab)
        {
            StartCoroutine(WaitAndBegin(_attackPreset, _attackInterval));
        }
    }

    void Update()
    {

    }

    IEnumerator WaitAndBegin(float time, float interval)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(WaitAndShoot(interval));
    }
    IEnumerator WaitAndShoot(float time)
    {
        _avatarAnimator.SetTrigger("Attack A");
        yield return new WaitForSeconds(0.25f);
        ShootBolt();
        yield return new WaitForSeconds(time - 0.25f);
        StartCoroutine(WaitAndShoot(_attackInterval));
    }

    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(1.5f);
        _boltAnimator.SetTrigger("Hit");
        _boltPrefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void ShootBolt()
    {
        var go = Instantiate(_boltPrefab, _parent);
        _boltAnimator = go.GetComponent<Animator>();
        go.transform.LookAt(transform.forward * 5f);
        go.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1f;
        go.GetComponent<Rigidbody>().velocity = transform.forward * 3f;
        //StartCoroutine(WaitAndExplode());
    }
}
