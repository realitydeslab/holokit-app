using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheTraining
{
    public enum AIAttackState
    {
        Nothing = 0,
        BasicSpell = 1,
        SecondarySpell = 2
    }

    public class MofaPlayerAI : MofaPlayer
    {
        [SerializeField] private MofaAvatarCollectionParamsList _mofaAvatarCollectionParamsList;

        [SerializeField] private RuntimeAnimatorController _mofaAvatarRuntimeAnimatorController;

        [SerializeField] private MagicSchool _magicSchool;

        private readonly NetworkVariable<Vector2> _animationVector = new(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private Animator _animator;

        private GameObject _avatar;

        // From avatar origin to avatar's center eye
        private Vector3 _centerEyeOffset;

        private MofaBaseRealityManager _mofaRealityManager;

        private Vector3 _initialPosition;

        private Vector3 _initialForward;

        private Vector3 _initialRight;

        private Vector3 _destPosition;

        private AIAttackState _lastAIAttackState;

        private Coroutine _attackAICoroutine;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        private bool _notFirstAnimationFrame;

        private Vector3 _lastFrameForward;

        private Vector3 _lastFrameRight;

        private Vector3 _lastFramePosition;

        private const float Speed = 0.3f;

        private readonly Vector4 VelocityRemap = new(-.02f, .02f, -1f, 1f);

        public static event Action<SpellType> OnAISpawnedSpell;

        protected override void Start()
        {
            base.Start();

            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _animationVector.OnValueChanged += OnAnimationVectorChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            _animationVector.OnValueChanged -= OnAnimationVectorChanged;
        }

        private void Update()
        {
            // For Chibi Apes local position bug
            if (_avatar != null)
            {
                _avatar.transform.localPosition = Vector3.zero;
            }
        }

        [ClientRpc]
        public void InitializeAvatarPositionClientRpc(Vector3 initialPosition, Quaternion initialRotation)
        {
            if (IsServer)
            {
                GameObject go = new();
                go.transform.SetPositionAndRotation(initialPosition, initialRotation);

                _initialPosition = go.transform.position;
                _initialForward = go.transform.forward;
                _initialRight = go.transform.right;

                _destPosition = go.transform.position;
                transform.position = go.transform.position;

                Destroy(go);
            }
            SpawnAvatar();
        }

        private void SpawnAvatar()
        {
            if (_avatar == null)
            {
                var preferencedAvatarCollection = HoloKitApp.Instance.GlobalSettings.GetPreferencedAvatarCollection(null);
                var preferencedAvatar = HoloKitApp.Instance.GlobalSettings.GetPreferencedAvatar(null);
                var avatarCollectionParams = _mofaAvatarCollectionParamsList.GetAvatarCollectionParams(preferencedAvatarCollection);
                _centerEyeOffset = avatarCollectionParams.CenterEyeOffset;
                LifeShieldOffset = avatarCollectionParams.LifeShiledOffset;
                _avatar = Instantiate(preferencedAvatar.Prefab,
                            transform.position, Quaternion.identity);
                // Setup avatar's components
                _avatar.transform.SetParent(transform);
                _avatar.transform.localPosition = Vector3.zero;
                _avatar.transform.localRotation = Quaternion.identity;
                _avatar.transform.localScale = new Vector3(avatarCollectionParams.Scale, avatarCollectionParams.Scale, avatarCollectionParams.Scale);
                _animator = _avatar.GetComponent<Animator>();
                _animator.runtimeAnimatorController = _mofaAvatarRuntimeAnimatorController;
            }
        }

        protected override void FixedUpdate()
        {
            // Update NetworkTransform
            if (IsServer)
            {
                // Rotation
                Vector3 forwardVector = HoloKitCamera.Instance.CenterEyePose.position - transform.position;
                if (forwardVector != Vector3.zero)
                {
                    transform.rotation = MofaUtils.GetHorizontalLookRotation(forwardVector);
                }

                // Position
                if (_mofaRealityManager.CurrentPhase == MofaPhase.Fighting)
                {
                    if (Vector3.Distance(transform.position, _destPosition) < 0.1f)
                    {
                        // Find a new destination position
                        GetNextDestPos();
                    }
                    else
                    {
                        // Approaching to the destination
                        transform.position += Speed * Time.fixedDeltaTime * (_destPosition - transform.position).normalized;
                        UpdateAvatarMovementAnimation();
                    }
                }
            }
        }

        private void GetNextDestPos()
        {
            float forwardVar = UnityEngine.Random.Range(-2.4f, 1.8f); // TODO: Adjust avatar movement area
            float rightVar = UnityEngine.Random.Range(-2.7f, 2.7f);

            _destPosition = _initialPosition + _initialForward * forwardVar + _initialRight * rightVar;
        }

        protected override void OnPhaseChanged(MofaPhase mofaPhase)
        {
            base.OnPhaseChanged(mofaPhase);

            if (IsServer)
            {
                if (mofaPhase == MofaPhase.Fighting)
                {
                    SetupSpellsForAI();
                    _attackAICoroutine = StartCoroutine(AttackAI());
                }
                else if (mofaPhase == MofaPhase.RoundOver)
                {
                    StopCoroutine(_attackAICoroutine);
                    if (IsServer)
                    {
                        _animationVector.Value = new Vector2(0f, 0f);
                    }
                }
            }
        }

        private void SetupSpellsForAI()
        {
            foreach (var spell in ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).SpellList.List)
            {
                if (spell.MagicSchool.TokenId.Equals(_magicSchool.TokenId))
                {
                    if (spell.SpellType == SpellType.Basic)
                    {
                        _basicSpell = spell;
                    }
                    else
                    {
                        _secondarySpell = spell;
                    }
                }
            }
        }

        private IEnumerator AttackAI()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));

            while (true)
            {
                float random = UnityEngine.Random.Range(0f, 1f);
                if (random < 0.2f) // Do nothing
                {
                    if (_lastAIAttackState == AIAttackState.Nothing)
                    {
                        StartCoroutine(SpawnSpellWithDelay(SpellType.Basic));
                    }
                    else
                    {
                        _lastAIAttackState = AIAttackState.Nothing;
                    }
                }
                else if (random > 0.9) // Secondary spell
                {
                    if (_lastAIAttackState == AIAttackState.SecondarySpell)
                    {
                        _lastAIAttackState = AIAttackState.Nothing;
                    }
                    else
                    {
                        StartCoroutine(SpawnSpellWithDelay(SpellType.Secondary));
                    }
                }
                else // Basic spell
                {
                    StartCoroutine(SpawnSpellWithDelay(SpellType.Basic));
                }
                yield return new WaitForSeconds(2.5f);
            }
        }

        private IEnumerator SpawnSpellWithDelay(SpellType spellType)
        {
            if (IsHostAlive() && IsAIAlive())
            {
                OnAISpawnedSpellClientRpc(spellType);
                if (spellType == SpellType.Basic)
                {
                    yield return new WaitForSeconds(0.3f);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
                SpawnSpell(spellType);
            }
        }

        private bool IsHostAlive()
        {
            var hostLifeShield = _mofaRealityManager.Players[0].LifeShield;
            if (hostLifeShield != null && !hostLifeShield.IsDestroyed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsAIAlive()
        {
            if (LifeShield != null && !LifeShield.IsDestroyed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [ClientRpc]
        private void OnAISpawnedSpellClientRpc(SpellType spellType)
        {
            OnAISpawnedSpell?.Invoke(spellType);
            PlayAvatarCastSpellAnimation(spellType);
        }

        private void SpawnSpell(SpellType spellType)
        {
            var hostLifeShield = _mofaRealityManager.Players[0].LifeShield;
            Vector3 avatarCenterEyePos = transform.position + transform.rotation * _centerEyeOffset;
            Quaternion rotation = Quaternion.LookRotation(hostLifeShield.transform.position - avatarCenterEyePos);
            // TODO: Random deviation
            if (UnityEngine.Random.Range(0, 1f) > 0.5f)
            {
                // Add horizontal deviation
                rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f) * rotation;
            }
            else
            {
                // Add vertical deviation
                rotation = Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), 0f, 0f) * rotation;
            }

            _mofaRealityManager.SpawnSpellServerRpc(spellType == SpellType.Basic ? _basicSpell.Id : _secondarySpell.Id,
                avatarCenterEyePos, rotation, OwnerClientId);
            _lastAIAttackState = spellType == SpellType.Basic ? AIAttackState.BasicSpell : AIAttackState.SecondarySpell;
        }

        private void UpdateAvatarMovementAnimation()
        {
            if (_animator == null)
            {
                return;
            }

            if (_notFirstAnimationFrame)
            {
                // Calculate the relative z and x velocity
                Vector3 distFromLastFrame = transform.position - _lastFramePosition;
                if (distFromLastFrame != Vector3.zero)
                {
                    float z = Vector3.Dot(distFromLastFrame, _lastFrameForward);
                    float x = Vector3.Dot(distFromLastFrame, _lastFrameRight);

                    var staticThreshold = 0.001667f; // if velocity < 0.1m/s, we regard it static.
                    z = MofaTrainingUtils.InverseClamp(z, -1 * staticThreshold, 1 * staticThreshold);
                    x = MofaTrainingUtils.InverseClamp(x, -1 * staticThreshold, 1 * staticThreshold);

                    z = MofaTrainingUtils.Remap(z, VelocityRemap.x, VelocityRemap.y, VelocityRemap.z, VelocityRemap.w, true);
                    x = MofaTrainingUtils.Remap(x, VelocityRemap.x, VelocityRemap.y, VelocityRemap.z, VelocityRemap.w, true);

                    _animationVector.Value = new Vector2(x, z);
                    //_animator.SetFloat("Velocity Z", z);
                    //_animator.SetFloat("Velocity X", x);
                }
            }
            else
            {
                _notFirstAnimationFrame = true;
            }

            // Save data for next frame calculation
            _lastFrameForward = transform.forward;
            _lastFrameRight = transform.right;
            _lastFramePosition = transform.position;
        }

        private void OnAnimationVectorChanged(Vector2 oldValue, Vector2 newValue)
        {
            _animator.SetFloat("Velocity Z", newValue.x);
            _animator.SetFloat("Velocity X", newValue.y);
        }

        private void PlayAvatarCastSpellAnimation(SpellType spellType)
        {
            if (spellType == SpellType.Basic)
            {

                _animator.SetTrigger("Attack A");
            }
            else
            {
                _animator.SetTrigger("Attack B");
            }
        }
    }
}
