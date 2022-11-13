using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class UnkaDragonController : NetworkBehaviour
    {
        [SerializeField] private MultiAimConstraint _headAimConstraint;

        [SerializeField] private MultiAimConstraint _chestAimConstraint;

        [SerializeField] private RigBuilder _rigBuilder;

        [SerializeField] Animator _animator;

        [SerializeField] private int _maxHealth = 30;

        private readonly NetworkVariable<bool> _isAttacking = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private Animator _dragonHeadTargetAnimator;

        [Header("Attack Behaviour")]
        [SerializeField] private GameObject _fireBallPrefab;

        [SerializeField] private GameObject _fireBreathPrefab;

        [SerializeField] private Transform _dragonMousePose;

        private GameObject _currentFireBall;

        private GameObject _currentFireBreath;

        [Header("Renderer")]
        [SerializeField] SkinnedMeshRenderer _dragonRenderer;

        [SerializeField] SkinnedMeshRenderer _eyeRenderer;

        [SerializeField] VisualEffect _dragonDeathVFX;

        public Vector3 ClipPlane
        {
            get => _clipPlane;
            set
            {
                _clipPlane = value;
            }
        }

        public float ClipPlaneHeihgt
        {
            get => _clipPlaneHeight;
            set
            {
                _clipPlaneHeight = value;
            }
        }

        private Vector3 _clipPlane = new(2f, 0f, -1f);

        private float _clipPlaneHeight = 3f;

        [Header("Test")]
        public GameObject DeathVFX;

        public bool Reset;

        public bool FireBall;

        public bool FireBreath;

        public bool AutoFireBall;

        public bool AutoFireBreath;

        private void Start()
        {
            _clipPlane = -transform.forward;
            _clipPlaneHeight = (transform.position + 2f * transform.forward).magnitude;
            SetRendererClipPlane();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetupHeadTarget();
            if (IsServer)
            {
                StartInitialMovement();
                _currentHealth.Value = _maxHealth;
            }
            _currentHealth.OnValueChanged += OnCurrentHealthValueChanged;
            _isAttacking.OnValueChanged += OnIsAttackingValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _currentHealth.OnValueChanged -= OnCurrentHealthValueChanged;
            _isAttacking.OnValueChanged -= OnIsAttackingValueChanged;
        }

        private void SetupHeadTarget()
        {
            // Get host head target
            Transform dragonHeadTarget = ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).DragonHeadTarget;
            // Set head aim
            var headData = _headAimConstraint.data.sourceObjects;
            headData.SetTransform(0, dragonHeadTarget);
            _headAimConstraint.data.sourceObjects = headData;
            // Set chest aim
            var chestData = _chestAimConstraint.data.sourceObjects;
            chestData.SetTransform(0, dragonHeadTarget);
            _chestAimConstraint.data.sourceObjects = chestData;
            // Rebuild
            _rigBuilder.Build();
            _dragonHeadTargetAnimator = dragonHeadTarget.GetComponent<Animator>();
        }

        private void StartInitialMovement()
        {
            LeanTween.move(gameObject, transform.position + 4f * transform.forward, 5f)
                .setOnComplete(() =>
                {
                    _isAttacking.Value = true;
                });
        }

        private void OnCurrentHealthValueChanged(int oldValue, int newValue)
        {
            if (oldValue > newValue)
            {
                Debug.Log($"[DragonController] Current health: {newValue}");
                if (newValue == 0)
                {
                    OnDeath();
                }
            }
        }

        private void OnIsAttackingValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                if (IsServer)
                {
                    // TODO: Start dragon attacking AI
                }
            }
        }

        private IEnumerator StartDragonAttackAI()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));

        }
        
        public void OnDamaged(int damage, ulong attackerClientId)
        {
            _currentHealth.Value -= damage;
        }

        private void OnDeath()
        {
            _animator.SetTrigger("Die");
            LeanTween.value(_clipPlaneHeight, -3f, 3f)
            .setOnUpdate((float value) =>
            {
                _clipPlaneHeight = value;
                SetRendererClipPlane();
            })
            .setOnComplete(() =>
            {
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            });
        }

        private void Update()
        {
            if (FireBall)
            {
                PlaySpawnFireBallAnimationClientRpc();
                FireBall = false;
            }

            if (FireBreath)
            {
                _animator.SetTrigger("Fire Breath");
                _dragonHeadTargetAnimator.SetTrigger("Fire Breath");
                FireBreath = false;
            }

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
        }

        private void SetRendererClipPlane()
        {
            var pos = _clipPlane;
            var plane = new Vector4(pos.x, pos.y, pos.z, _clipPlaneHeight);
            foreach (var material in _dragonRenderer.materials)
            {
                material.SetVector("_Clip_Plane", plane);
            }
            _eyeRenderer.material.SetVector("_Clip_Plane", plane);
            _dragonDeathVFX.SetVector4("Clip Plane", plane);
        }

        [ClientRpc]
        private void PlaySpawnFireBallAnimationClientRpc()
        {
            _animator.SetTrigger("Fire Ball");
        }

        public void OnSpawnFireBall()
        {
            if (IsServer)
            {
                _currentFireBall = Instantiate(_fireBallPrefab, _dragonMousePose.position, _dragonMousePose.rotation);
                _currentFireBall.GetComponent<FireBallController>().DragonMousePose = _dragonMousePose;
                _currentFireBall.GetComponent<NetworkObject>().SpawnWithOwnership(999);
            }
        }

        public void OnFireBallAttack()
        {
            if (IsServer)
            {
                _currentFireBall.GetComponent<FireBallController>().FlyToTarget();
            }
        }

        public void OnFireBreathInit()
        {
            _currentFireBreath = Instantiate(_fireBreathPrefab);
            _currentFireBreath.GetComponent<FireBreathController>().followPoint = _dragonMousePose;
        }

        public void OnFireBreathAttack()
        {
            _currentFireBreath.GetComponent<FireBreathController>().OnAttack();
        }

        //private void OnDrawGizmos()
        //{
        //    var dir = (_enemyPoint == null ? new Vector3(0, -2, 2) : _enemyPoint.position - _powerInitPoint.position).normalized;
        //    Debug.DrawRay(_powerInitPoint.position, dir*10f, Color.red);
        //}

        public void OnAnimationStop()
        {
            _animator.StopPlayback();
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