using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.MOFABase;
using HoloKit;

namespace Holoi.Reality.MOFATheTraining
{
    /// <summary>
    /// This part of the class is responsible for controlling the state machine.
    /// </summary>
    public partial class MofaAIPlayer
    {
        public MofaAIPlayerState IdleState = new MofaAIPlayerState_Idle();
        public MofaAIPlayerState MovementState = new MofaAIPlayerState_Movement();
        public MofaAIPlayerState AttackState = new MofaAIPlayerState_Attack();
        public MofaAIPlayerState DashState = new MofaAIPlayerState_Dash();
        public MofaAIPlayerState DamageState = new MofaAIPlayerState_Damage();
        public MofaAIPlayerState RevivingState = new MofaAIPlayerState_Reviving();

        /// <summary>
        /// The avatar's spawn position.
        /// </summary>
        public Vector3 InitialPosition;

        /// <summary>
        /// The initial forward when the avatar is spawned.
        /// </summary>
        public Vector3 InitialForward;

        /// <summary>
        /// The current state the avatar is in.
        /// </summary>
        private MofaAIPlayerState _currentState;

        /// <summary>
        /// We only need to init state machine on the host.
        /// </summary>
        private void InitStateMachine()
        {
            _currentState = IdleState;
            _currentState.OnEnter(this);
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;
            LifeShield.OnBeingHit += OnBeingHit;
        }

        private void DeinitStateMachine()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
            LifeShield.OnBeingHit -= OnBeingHit;
        }

        private void OnMofaPhaseChanged(MofaPhase phase)
        {
            if (phase == MofaPhase.Fighting)
            {
                SwitchState(MovementState);
                return;
            }

            if (phase == MofaPhase.RoundOver)
            {
                SwitchState(IdleState);
                return;
            }
        }

        public void SwitchState(MofaAIPlayerState state)
        {
            _currentState.OnExit(this);
            _currentState = state;
            _currentState.OnEnter(this);
        }

        private void UpdateStateMachine()
        {
            _currentState.OnUpdate(this);
        }

        private void OnBeingHit(ulong _, ulong clientId)
        {
            if (clientId == AIClientId)
                SwitchState(DamageState);
        }
    }
}
