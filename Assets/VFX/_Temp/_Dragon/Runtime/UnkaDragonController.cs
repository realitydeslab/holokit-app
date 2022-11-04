using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnkaDragonController : MonoBehaviour
{
    [Header("Attack Behaviour")]
    [SerializeField] Animator _aniamtor;
    [SerializeField] GameObject _fireBallPrefab;
    [SerializeField] Transform _InitPoint;
    GameObject _fireBallInstance;
    [SerializeField] Transform _enemyPoint;
    float _vX=0;
    float _vZ=0;

    [Header("Test")]
    public bool Reset;
    public bool Die;
    public bool FireBall;
    public bool FireBreath;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
            FireBreath = false;
        }

        if (Reset)
        {
            _aniamtor.Rebind();
            _aniamtor.Update(0);
            Reset = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _vZ += Time.deltaTime;
            if (_vZ > 1) _vZ = 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _vZ -= Time.deltaTime;
            if (_vZ < 0) _vZ = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _vX += Time.deltaTime;
            if (_vX > 1) _vX = 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _vX -= Time.deltaTime;
            if (_vX < 0) _vX = 0;
        }
    }

    public void OnFireBallAttack()
    {
        Debug.Log("OnFireBallAttack");
        _fireBallInstance = Instantiate(_fireBallPrefab);
        _fireBallInstance.transform.position = _InitPoint.position;
        var dir = (_enemyPoint==null?new Vector3(0,-2,2):_enemyPoint.position - _InitPoint.position).normalized;
        var speed = dir * 3f;
        _fireBallInstance.GetComponent<Rigidbody>().velocity = speed;
    }

    public void OnFireBreathAttack()
    {
        Debug.Log("OnFireBreathAttack");
        //_fireBallInstance = Instantiate(_fireBallPrefab);
        //_fireBallInstance.transform.position = _InitPoint.position;
        //var dir = (_enemyPoint == null ? new Vector3(0, -2, 2) : _enemyPoint.position - _InitPoint.position).normalized;
        //var speed = dir * 3f;
        //_fireBallInstance.GetComponent<Rigidbody>().velocity = speed;
    }

    private void OnDrawGizmos()
    {
        var dir = (_enemyPoint == null ? new Vector3(0, -2, 2) : _enemyPoint.position - _InitPoint.position).normalized;
        Debug.DrawRay(_InitPoint.position, dir*10f, Color.red);
    }

    public void PlaySound()
    {

    }
}
