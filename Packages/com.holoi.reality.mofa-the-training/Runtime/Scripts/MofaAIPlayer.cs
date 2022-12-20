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
        None = 0,
        BasicSpell = 1,
        SecondarySpell = 2
    }

    public partial class MofaAIPlayer : MofaPlayer
    {
        private Spell _basicSpell;

        private Spell _secondarySpell;

        private const float Speed = 0.3f;

        public const ulong AIClientId = 101;

        public static event Action<SpellType> OnAISpawnedSpell;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                InitStateMachine();
                InitAnimationControl();
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                DeinitStateMachine();
                DeinitAnimationControl();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (IsSpawned && IsServer)
            {
                LootAtTarget();
                UpdateStateMachine();
            }
        }

        /// <summary>
        /// The avatar should always look at the player.
        /// </summary>
        private void LootAtTarget()
        {
            Vector3 lookForward = (HoloKitCamera.Instance.CenterEyePose.position - transform.position);
            Vector3 horizontalLookForward = Vector3.ProjectOnPlane(lookForward, Vector3.up);
            if (horizontalLookForward == Vector3.zero)
                return;
            Quaternion rotation = Quaternion.LookRotation(horizontalLookForward);
            transform.rotation = rotation;
        }

        private void SpawnSpell(SpellType spellType)
        {
            //var hostLifeShield = _mofaBaseRealityManager.PlayerDict[0].LifeShield;
            //Vector3 avatarCenterEyePos = transform.position + transform.rotation * _centerEyeOffset;
            //Quaternion rotation = Quaternion.LookRotation(hostLifeShield.transform.position - avatarCenterEyePos);
            //// TODO: Random deviation
            //if (UnityEngine.Random.Range(0, 1f) > 0.5f)
            //{
            //    // Add horizontal deviation
            //    rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f) * rotation;
            //}
            //else
            //{
            //    // Add vertical deviation
            //    rotation = Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), 0f, 0f) * rotation;
            //}

            //_mofaBaseRealityManager.SpawnSpellServerRpc(spellType == SpellType.Basic ? _basicSpell.Id : _secondarySpell.Id,
            //    avatarCenterEyePos, rotation, OwnerClientId);
            //_lastAIAttackState = spellType == SpellType.Basic ? AIAttackState.BasicSpell : AIAttackState.SecondarySpell;
        }
    }
}
