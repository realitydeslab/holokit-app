using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;
using HoloKit;

namespace Holoi.Reality.MOFATheTraining
{
    /// <summary>
    /// This part of the class is responsible for controlling the state machine.
    /// </summary>
    public partial class MofaAIPlayer
    {
        public MofaAIPlayerState_Idle IdleState = new();
        public MofaAIPlayerState_Movement MovementState = new();
        public MofaAIPlayerState_Attack AttackState = new();
        public MofaAIPlayerState_Dash DashState = new();
        public MofaAIPlayerState_Damage DamageState = new();
        public MofaAIPlayerState_Revive ReviveState = new();

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
            LifeShield.OnBeingDestroyed += OnKnockedOut;
            LifeShield.OnRenovated += OnRevived;
        }

        private void DeinitStateMachine()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
            LifeShield.OnBeingHit -= OnBeingHit;
            LifeShield.OnBeingDestroyed -= OnKnockedOut;
            LifeShield.OnRenovated -= OnRevived;
        }

        public void SwitchState(MofaAIPlayerState state)
        {
            if (_currentState == state)
                return;

            _currentState.OnExit(this);
            _currentState = state;
            _currentState.OnEnter(this);
        }

        private void UpdateStateMachine()
        {
            _currentState.OnUpdate(this);
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

        private void OnBeingHit(ulong _, ulong clientId)
        {
            if (clientId == AIClientId)
                SwitchState(DamageState);
        }

        private void OnKnockedOut(ulong _, ulong clientId)
        {
            if (clientId == AIClientId)
                SwitchState(IdleState);
        }

        private void OnRevived(ulong clientId)
        {
            if (clientId == AIClientId)
            {
                var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                if (mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Fighting)
                {
                    SwitchState(MovementState);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<AttackSpell>(out var attackSpell) && attackSpell.OwnerClientId != AIClientId)
            {
                if (_currentState == MovementState)
                {
                    if (DashState.ShouldDash())
                    {
                        SwitchState(DashState);
                    }
                }
            }
        }
    }
}
