using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

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

    [Header("Renderer")]
    [SerializeField] SkinnedMeshRenderer _dragonRenderer;

    [SerializeField] SkinnedMeshRenderer _eyeRenderer;

    [SerializeField] VisualEffect _dragonDeathVFX;

    [SerializeField] Vector3  _clipPlane = new Vector3(2,0,-1);

    [SerializeField] float  _clipPlaneProcess = 3f;

    bool _isDuringDeath = false;

    [Header("Test")]
    public GameObject DeathVFX;

    public bool Reset;

    public bool Die;

    public bool FireBall;

    public bool FireBreath;

    public bool AutoFireBall;

    public bool AutoFireBreath;

    void Start()
    {
        if(AutoFireBall) 
        StartCoroutine(WaitAndFireBall());
        if(AutoFireBreath)
        StartCoroutine(WaitAndFireBreath());
    }

    void Update()
    {
        if (Die)
        {
            _aniamtor.SetTrigger("Die");
            Die = false;

            OnDeath();
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
            _clipPlaneProcess = 3f;

            var plane = new Vector4(_clipPlane.x, _clipPlane.y, _clipPlane.z, _clipPlaneProcess);

            foreach (var material in _dragonRenderer.materials)
            {
                material.SetVector("_Clip_Plane", plane);
            }

            _eyeRenderer.material.SetVector("_Clip_Plane", plane);

            _dragonDeathVFX.SetVector4("Clip Plane", plane);

            _aniamtor.Rebind();
            _aniamtor.Update(0);
            Reset = false;
        }

        if(_isDuringDeath)
        {
            UpdateRendererClipPlaneDuraingDeathAnimation();
        }

        if (true)
        {
            SetRendereClip();
        }
    }

    void UpdateRendererClipPlaneDuraingDeathAnimation()
    {
        _clipPlaneProcess -= Time.deltaTime * 3f;

        if (_clipPlaneProcess < -3f)
        {
            _clipPlaneProcess = -3f;
            _isDuringDeath = false;
        }
    }

    void SetRendereClip()
    {
        //var pos = _clipPlane + transform.position;

        var plane = new Vector4(_clipPlane.x, _clipPlane.y, _clipPlane.z, _clipPlaneProcess);

        foreach (var material in _dragonRenderer.materials)
        {
            material.SetVector("_Clip_Plane", plane);
        }

        _eyeRenderer.material.SetVector("_Clip_Plane", plane);

        _dragonDeathVFX.SetVector4("Clip Plane", plane);
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
        _isDuringDeath = true;
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

    IEnumerator WaitAndFireBall()
    {
        FireBall = true;
        yield return new WaitForSeconds(4.5f);
        StartCoroutine(WaitAndFireBall());
    }

    IEnumerator WaitAndFireBreath()
    {
        FireBreath = true;
        yield return new WaitForSeconds(10f);
        StartCoroutine(WaitAndFireBreath());
    }
}
