using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Reality.MOFATheTraining
{
    /// <summary>
    /// This part of the class is responsible for controlling the state machine.
    /// </summary>
    public partial class MofaAIPlayer
    {
        /// <summary>
        /// The current state the avatar is in.
        /// </summary>
        private MofaAIPlayerState _currentState;

        private void InitializeStateMachine()
        {
            
        }

        public void SwitchState(MofaAIPlayerState state)
        {
            _currentState.OnExit(this);
            _currentState = state;
            _currentState.OnEnter(this);
        }

        private void UpdateStateMachine()
        {
            if (!IsSpawned || !IsServer)
                return;

            LootAtTarget();

            _currentState.OnUpdate(this);
        }

        /// <summary>
        /// The avatar should always look at the player.
        /// </summary>
        private void LootAtTarget()
        {
            Vector3 lookForward = (HoloKitCamera.Instance.CenterEyePose.position - transform.position);
            Vector3 horizontalLookForward = Vector3.ProjectOnPlane(lookForward, Vector3.up);
            Quaternion rotation = Quaternion.LookRotation(horizontalLookForward);
            transform.rotation = rotation;
        }
    }
}
