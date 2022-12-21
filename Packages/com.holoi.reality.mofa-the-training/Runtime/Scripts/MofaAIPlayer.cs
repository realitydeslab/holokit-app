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
    }
}
