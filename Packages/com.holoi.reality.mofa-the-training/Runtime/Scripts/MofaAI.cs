using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.AssetFoundation;
using HoloKit;
using Holoi.Mofa.Base;
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

        private GameObject _avatar;

        private MofaBaseRealityManager _mofaRealityManager;

        private Vector3 _initialPos;

        private Vector3 _destPos;

        private float _speed = 0.3f;

        private AIAttackState _lastAIAttackState;

        private Coroutine _attackAICoroutine;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        public static event Action<SpellType> OnAISpawnedSpell;

        protected override void Awake()
        {
            base.Awake();

            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Spawn avatar on each client locally
            SpawnAvatar();

            if (IsServer)
            {
                var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                mofaRealityManager.SpawnLifeShield(OwnerClientId);

                _initialPos = transform.position;
                _destPos = transform.position;
            }
        }

        private void SpawnAvatar()
        {
            if (_avatar == null)
            {
                _avatar = Instantiate(AvatarList.list[0].metaAvatars[0].prefab,
                transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("[AvatarController] Avatar already spawned");
            }
        }

        protected override void FixedUpdate()
        {
            // Update NetworkTransform
            if (IsServer)
            {
                // Rotation
                Vector3 lookAtVector = HoloKitCamera.Instance.CenterEyePose.position - transform.position;
                if (lookAtVector != Vector3.zero)
                {
                    Quaternion lookAtRotation = Quaternion.LookRotation(lookAtVector);
                    transform.rotation = MofaUtils.GetHorizontalRotation(lookAtRotation);
                }

                // Position
                if (_mofaRealityManager.Phase.Value == MofaPhase.Fighting)
                {
                    if (Vector3.Distance(transform.position, _destPos) < 0.1f)
                    {
                        // Find a new destination position
                        GetNextDestPos();
                    }
                    else
                    {
                        // Approaching to the destination
                        transform.position += _speed * Time.fixedDeltaTime * (_destPos - transform.position).normalized;
                    }
                }
            }

            // Set the avatar's transform on each client manually
            if (_avatar != null)
            {
                _avatar.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            // Set the life shield's transform on each client manually
            if (LifeShield != null)
            {
                LifeShield.transform.SetPositionAndRotation(transform.position + transform.rotation * new Vector3(0f, 1f, 0.8f),
                    transform.rotation);
            }
        }

        private void GetNextDestPos()
        {
            var horizontalForward = MofaUtils.GetHorizontalForward(transform);
            var horizontalRight = MofaUtils.GetHorizontalRight(transform);
            float forwardVar = UnityEngine.Random.Range(-3f, 3f);
            float rightVar = UnityEngine.Random.Range(-3f, 3f);

            _destPos = _initialPos + horizontalForward * forwardVar + horizontalRight * rightVar;
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
    }
}
