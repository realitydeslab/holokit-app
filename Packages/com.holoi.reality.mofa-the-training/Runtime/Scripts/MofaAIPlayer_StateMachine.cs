using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

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
        [HideInInspector] public Vector3 InitialPosition;

        /// <summary>
        /// The initial forward when the avatar is spawned.
        /// </summary>
        [HideInInspector] public Vector3 InitialForward;

        public SpellType NextSpellType { get; set; }

        /// <summary>
        /// The current state the avatar is in.
        /// </summary>
        private MofaAIPlayerState _currentState;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        /// <summary>
        /// We only need to init state machine on the host.
        /// </summary>
        private void InitStateMachine()
        {
            SetupSpells();
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

        private void SetupSpells()
        {
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            int count = 0;
            foreach (var spell in mofaBaseRealityManager.SpellList.List)
            {
                int spellMagicSchoolIndex = int.Parse(spell.MagicSchool.TokenId);
                if (spellMagicSchoolIndex == MagicSchoolIndex.Value)
                {
                    if (spell.SpellType == SpellType.Basic)
                    {
                        _basicSpell = spell;
                        count++;
                    }
                    else if (spell.SpellType == SpellType.Secondary)
                    {
                        _secondarySpell = spell;
                        count++;
                    }
                }

                if (count == 2)
                    break;
            }
            // Error check
            if (_basicSpell == null)
                Debug.LogError("[MofaAIPlayer] Failed to setup basic spell");
            if (_secondarySpell == null)
                Debug.LogError("[MofaAIPlayer] Failed to setup secondary spell");
        }

        private void CastSpell()
        {
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var hostLifeShield = mofaBaseRealityManager.HostMofaPlayer.LifeShield;
            Vector3 avatarCenterEyePosition = transform.position + transform.rotation * _avatarOriginToCenterEyeOffset;
            Quaternion spellRotation = Quaternion.LookRotation(hostLifeShield.transform.position - avatarCenterEyePosition);

            // Add random deviations
            if (NextSpellType == SpellType.Basic)
            {
                // Rotate around Y axis
                Quaternion horizontalDeviation = Quaternion.Euler(0f, Random.Range(-10f, 10f), 0f);
                // Rotate around X axis
                Quaternion verticalDeviation = Quaternion.Euler(Random.Range(-10f, 10f), 0f, 0f);
                spellRotation = horizontalDeviation * verticalDeviation * spellRotation;
            }
            else if (NextSpellType == SpellType.Secondary)
            {
                // Rotate around Y axis
                Quaternion horizontalDeviation = Quaternion.Euler(0f, Random.Range(-6f, 6f), 0f);
                spellRotation = horizontalDeviation * spellRotation;
            }

            mofaBaseRealityManager.SpawnSpellServerRpc(NextSpellType == SpellType.Basic ? _basicSpell.Id : _secondarySpell.Id,
                avatarCenterEyePosition, spellRotation, AIClientId);
        }
    }
}
