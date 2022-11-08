using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnkaDragonController : MonoBehaviour
{
    [Header("Attack Behaviour")]
    [SerializeField] Animator _aniamtor;

    [SerializeField] GameObject _fireBallPrefab;

    [SerializeField] GameObject _fireBreathPrefab;

    [SerializeField] Transform _powerInitPoint;

    GameObject _fireBallInstance;

    GameObject _fireBreathInstance;

    [SerializeField] Transform _enemyTarget;

    [SerializeField] Animator _targetAnimator;

    [Header("Test")]
    public GameObject DeathVFX;
    public bool Reset;

    public bool Die;

    public bool FireBall;

    public bool FireBreath;

    void Start()
    {
        StartCoroutine(WaitAndFire());
    }

    void Update()
    {
        if (Die)
        {
            _aniamtor.SetTrigger("Die");
            Die = false;
        }

        if (FireBall)
        {
            _aniamtor.SetTrigger("Fire Ball");
            FireBall = false;
        }


        if (FireBreath)
        {
            _aniamtor.SetTrigger("Fire Breath");
            _targetAnimator.SetTrigger("Fire Breath");
            FireBreath = false;
        }

        if (Reset)
        {
            _aniamtor.Rebind();
            _aniamtor.Update(0);
            Reset = false;
        }
    }

    public void OnFireBallInit()
    {
        _fireBallInstance = Instantiate(_fireBallPrefab);
        _fireBallInstance.GetComponent<FireBreathController>()._followPoint = _powerInitPoint;
    }

    public void OnFireBallAttack()
    {
        _fireBallInstance.GetComponent<FireBreathController>().IsFollow = false;
        var dir = (_enemyTarget == null ? new Vector3(0, -2, 2) : _enemyTarget.position - _powerInitPoint.position).normalized;
        var speed = dir * 3f;
        _fireBallInstance.GetComponent<Rigidbody>().velocity = speed;
    }

    public void OnFireBreathInit()
    {
        Debug.Log("OnFireBreathInit");
        _fireBreathInstance = Instantiate(_fireBreathPrefab);
        _fireBreathInstance.GetComponent<FireBreathController>()._followPoint = _powerInitPoint;
    }

    public void OnFireBreathAttack()
    {
        _fireBreathInstance.GetComponent<FireBreathController>().OnAttack();
    }

    //private void OnDrawGizmos()
    //{
    //    var dir = (_enemyPoint == null ? new Vector3(0, -2, 2) : _enemyPoint.position - _powerInitPoint.position).normalized;
    //    Debug.DrawRay(_powerInitPoint.position, dir*10f, Color.red);
    //}
    public void OnDeath()
    {
        DeathVFX.SetActive(true);
        StartCoroutine(WaitAndDisableGameObject(DeathVFX, 4f));
    }

    public void OnAnimationStop()
    {
        _aniamtor.StopPlayback();
    }

    IEnumerator WaitAndDisableGameObject(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(false);
    }

    public void PlaySound()
    {

    }

    IEnumerator WaitAndFire()
    {
        FireBall = true;
        yield return new WaitForSeconds(4.5f);
        FireBreath = true;
        yield return new WaitForSeconds(4.5f);
        StartCoroutine(WaitAndFire());
    }
}
