// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFA.TheTraining
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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                InitMofaAIPlayerInfo();
                InitStateMachine();
            }
            InitAnimationControl();
        }

        private void InitMofaAIPlayerInfo()
        {
            //Initialize NetworkVariables for HoloKitAppPlayer
            PlayerName.Value = "AI";
            PlayerType.Value = HoloKitAppPlayerType.Player;
            PlayerStatus.Value = HoloKitAppPlayerStatus.Checked;
            // Initialize NetworkVariables for MofaPlayer
            Team.Value = MofaTeam.Red;
            MagicSchoolIndex.Value = 0;
            Ready.Value = true;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                DeinitStateMachine();
            }
            DeinitAnimationControl();
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
            Vector3 lookForward = (HoloKitCameraManager.Instance.CenterEyePose.position - transform.position);
            Vector3 horizontalLookForward = Vector3.ProjectOnPlane(lookForward, Vector3.up);
            if (horizontalLookForward == Vector3.zero)
                return;
            Quaternion rotation = Quaternion.LookRotation(horizontalLookForward);
            transform.rotation = rotation;
        }
    }
}
