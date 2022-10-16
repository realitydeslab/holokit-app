using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.AssetFoundation;
using HoloKit;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;
using System;

namespace Holoi.Reality.MOFATheTraining
{
    public enum AIAttackState
    {
        Nothing = 0,
        BasicSpell = 1,
        SecondarySpell = 2
    }

    public class MofaAI : MofaPlayer
    {
        public MetaAvatarCollectionList AvatarList;

        public SpellList SpellList;

        [SerializeField] private RuntimeAnimatorController _mofaAvatarRuntimeAnimatorController;

        private Animator _animator;

        private GameObject _avatar;

        private MofaBaseRealityManager _mofaRealityManager;

        private Vector3 _initialPosition;

        private Vector3 _initialForward;

        private Vector3 _initialRight;

        private Vector3 _destPosition;

        private readonly float _speed = 0.3f;

        private AIAttackState _lastAIAttackState;

        private Coroutine _attackAICoroutine;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        private bool _notFirstAnimationFrame;

        private Vector3 _lastFrameForward;

        private Vector3 _lastFrameRight;

        private Vector3 _lastFramePosition;

        private readonly Vector4 _velocityRemap = new(-.02f, .02f, -1f, 1f);

        public static event Action<SpellType> OnAISpawnedSpell;

        protected override void Awake()
        {
            base.Awake();

            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("[MofaAI] OnNetworkSpawn");

            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
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
                _avatar = Instantiate(AvatarList.List[0].MetaAvatars[0].Prefab,
                            transform.position, Quaternion.identity);
                // Setup avatar's components
                _avatar.transform.SetParent(transform);
                _avatar.transform.localPosition = Vector3.zero;
                _avatar.transform.localRotation = Quaternion.identity;
                _animator = _avatar.GetComponent<Animator>();
                _animator.runtimeAnimatorController = _mofaAvatarRuntimeAnimatorController;
            }
        }

        private void OnNetworkTick()
        {
            if (!IsServer)
            {
                UpdateAvatarMovementAnimation();
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
                if (_mofaRealityManager.Phase.Value == MofaPhase.Fighting)
                {
                    if (Vector3.Distance(transform.position, _destPosition) < 0.1f)
                    {
                        // Find a new destination position
                        GetNextDestPos();
                    }
                    else
                    {
                        // Approaching to the destination
                        transform.position += _speed * Time.fixedDeltaTime * (_destPosition - transform.position).normalized;
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
                }
            }
        }

        private void SetupSpellsForAI()
        {
            foreach (var spell in SpellList.List)
            {
                if (spell.MagicSchool.Id == 0)
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

            while(true)
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
                else if (random > 0.8) // Secondary spell
                {

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

        [ClientRpc]
        private void OnAISpawnedSpellClientRpc(SpellType spellType)
        {
            OnAISpawnedSpell?.Invoke(spellType);
            PlayAvatarCastSpellAnimation(spellType);
        }
        
        private void SpawnSpell(SpellType spellType)
        {
            var hostLifeShield = _mofaRealityManager.Players[0].LifeShield;
            if (LifeShield != null && hostLifeShield != null)
            {
                Vector3 avatarCenterEyePos = transform.position + transform.rotation * new Vector3(0f, 1.2f, 1f); // TODO: Give a better offset
                Quaternion rotation = Quaternion.LookRotation(hostLifeShield.transform.position - avatarCenterEyePos);
                // TODO: Random deviation

                _mofaRealityManager.SpawnSpellServerRpc(spellType == SpellType.Basic ? _basicSpell.Id : _secondarySpell.Id,
                    avatarCenterEyePos, rotation, OwnerClientId);
                _lastAIAttackState = spellType == SpellType.Basic ? AIAttackState.BasicSpell : AIAttackState.SecondarySpell;
            }
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

                    z = MofaTrainingUtils.Remap(z, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);
                    x = MofaTrainingUtils.Remap(x, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);

                    _animator.SetFloat("Velocity Z", z);
                    _animator.SetFloat("Velocity X", x);
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
