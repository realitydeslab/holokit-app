using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class ThumbnailDragonController : MonoBehaviour
{
    [SerializeField] GameObject _fireBallPrefab;
    [SerializeField] Transform _attackTarget;
    GameObject _currentFireBall;
    [SerializeField] Transform _dragonMousePose;
    Animator _animator;
    bool _isFireBallCharging = false;

    // debug: 
    public bool _fireBall;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(WaitAndFire());
    }

    // Update is called once per frame
    void Update()
    {
        if (_fireBall)
        {
            _animator.SetTrigger("Fire Ball");
            _fireBall = false;
        }

        if (_isFireBallCharging && _currentFireBall)
        {
            _currentFireBall.transform.position = _dragonMousePose.position;
        }
    }

    public void OnSpawnFireBall()
    {
        _isFireBallCharging = true;
        _currentFireBall = Instantiate(_fireBallPrefab, _dragonMousePose.position, _dragonMousePose.rotation);
        Destroy(_currentFireBall, 8f);
    }

    public void OnFireBallAttack()
    {
        _isFireBallCharging = false;
        var dir = (_attackTarget.position - _dragonMousePose.position).normalized;
        var speed = dir * 4f;
        _currentFireBall.GetComponent<Rigidbody>().velocity = speed;
    }

    public void PlaySound()
    {

    }

    IEnumerator WaitAndFire()
    {
        _animator.SetTrigger("Fire Ball");
        yield return new WaitForSeconds(2f);
        StartCoroutine(WaitAndFire());
    }
}
