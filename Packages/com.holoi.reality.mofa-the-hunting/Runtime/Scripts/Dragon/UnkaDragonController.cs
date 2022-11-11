using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting
{
    [RequireComponent(typeof(MovementController))]
    public class UnkaDragonController : NetworkBehaviour
    {
        [SerializeField] private MultiAimConstraint _headAimConstraint;

        [SerializeField] private MultiAimConstraint _chestAimConstraint;

        [SerializeField] private RigBuilder _rigBuilder;

        [Header("Attack Behaviour")]
        [SerializeField] Animator _aniamtor;

        [SerializeField] GameObject _fireBallPrefab;

        [SerializeField] GameObject _fireBreathPrefab;

        [SerializeField] Transform _powerInitPoint;

        GameObject _fireBallInstance;

        GameObject _fireBreathInstance;

        Transform _attackTarget;

        [SerializeField] Animator _targetAnimator;

        [Header("Renderer")]
        [SerializeField] SkinnedMeshRenderer _dragonRenderer;

        [SerializeField] SkinnedMeshRenderer _eyeRenderer;

        [SerializeField] VisualEffect _dragonDeathVFX;

        [SerializeField] Vector3 _clipPlane = new Vector3(2, 0, -1);

        public Vector3 ClipPlane
        {
            set { _clipPlane = value; }
            get { return _clipPlane; }
        }

        [SerializeField] float _clipPlaneHeight = 3f;

        public float ClipPlaneHeihgt
        {
            set { _clipPlaneHeight = value; }
            get { return _clipPlaneHeight; }
        }

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
            //_attackTarget = HoloKit.HoloKitCamera.Instance.CenterEyePose;

            //if (AutoFireBall)
            //    StartCoroutine(WaitAndFireBall());
            //if (AutoFireBreath)
            //    StartCoroutine(WaitAndFireBreath());
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetupHeadTarget();
            if (IsServer)
            {
                StartInitialMovement();
            }
        }

        private void SetupHeadTarget()
        {
            // Get host head target
            Transform headTarget = ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).HeadTarget;
            // Set head aim
            var headData = _headAimConstraint.data.sourceObjects;
            headData.SetTransform(0, headTarget);
            _headAimConstraint.data.sourceObjects = headData;
            // Set chest aim
            var chestData = _chestAimConstraint.data.sourceObjects;
            chestData.SetTransform(0, headTarget);
            _chestAimConstraint.data.sourceObjects = chestData;
            // Rebuild
            _rigBuilder.Build();
        }

        private void StartInitialMovement()
        {
            LeanTween.move(gameObject, transform.position + 4f * transform.forward, 5f)
                .setOnComplete(() =>
                {
                    // Start enemy behaviour
                });
        }

        private void Update()
        {

            //if (Die)
            //{
            //    _aniamtor.SetTrigger("Die");
            //    Die = false;
            //    OnDeath();
            //}

            //if (FireBall)
            //{
            //    _aniamtor.SetTrigger("Fire Ball");
            //    FireBall = false;
            //}


            //if (FireBreath)
            //{
            //    _aniamtor.SetTrigger("Fire Breath");
            //    _targetAnimator.SetTrigger("Fire Breath");
            //    FireBreath = false;
            //}

            //if (Reset)
            //{
            //    _clipPlaneHeight = 3f;

            //    var plane = new Vector4(_clipPlane.x, _clipPlane.y, _clipPlane.z, _clipPlaneHeight);

            //    foreach (var material in _dragonRenderer.materials)
            //    {
            //        material.SetVector("_Clip_Plane", plane);
            //    }

            //    _eyeRenderer.material.SetVector("_Clip_Plane", plane);

            //    _dragonDeathVFX.SetVector4("Clip Plane", plane);

            //    _aniamtor.Rebind();
            //    _aniamtor.Update(0);
            //    Reset = false;
            //}

            //if (_isDuringDeath)
            //{
            //    UpdateRendererClipPlaneDuraingDeathAnimation();
            //}

            if (true)
            {
                SetRendereClip();
            }
        }

        void UpdateRendererClipPlaneDuraingDeathAnimation()
        {
            _clipPlaneHeight -= Time.deltaTime * 3f;

            if (_clipPlaneHeight < -3f)
            {
                _clipPlaneHeight = -3f;
                _isDuringDeath = false;
            }
        }

        void SetRendereClip()
        {
            var pos = _clipPlane;

            var worldPos = _dragonRenderer.transform.InverseTransformPoint(_clipPlane);

            var plane = new Vector4(pos.x, pos.y, pos.z, _clipPlaneHeight);



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
            _fireBallInstance.GetComponent<FireBreathController>().followPoint = _powerInitPoint;
        }

        public void OnFireBallAttack()
        {
            _fireBallInstance.GetComponent<FireBreathController>().IsFollow = false;
            var dir = (_attackTarget == null ? new Vector3(0, -2, 2) : _attackTarget.position - _powerInitPoint.position).normalized;
            var speed = dir * 3f;
            _fireBallInstance.GetComponent<Rigidbody>().velocity = speed;
        }

        public void OnFireBreathInit()
        {
            Debug.Log("OnFireBreathInit");
            _fireBreathInstance = Instantiate(_fireBreathPrefab);
            _fireBreathInstance.GetComponent<FireBreathController>().followPoint = _powerInitPoint;
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
}