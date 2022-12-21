using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheTraining
{
    /// <summary>
    /// This part of the class is responsible for controlling the animation of the avatar.
    /// </summary>
    public partial class MofaAIPlayer
    {
        /// <summary>
        /// Velocity.x for VelocityX and Velocity.y for Velocity.z.
        /// </summary>
        public NetworkVariable<Vector2> Velocity = new(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private int VelocityXHash;

        private int VelocityZHash;

        [HideInInspector] public int ActionHash;

        [HideInInspector] public int TriggerNumberHash;

        [HideInInspector] public int TriggerHash;

        private void InitAnimationControl()
        {
            VelocityXHash = Animator.StringToHash("VelocityX");
            VelocityZHash = Animator.StringToHash("VelocityZ");
            ActionHash = Animator.StringToHash("Action");
            TriggerNumberHash = Animator.StringToHash("TriggerNumber");
            TriggerHash = Animator.StringToHash("Trigger");
            Velocity.OnValueChanged += OnVelocityValueChanged;
        }

        private void DeinitAnimationControl()
        {
            Velocity.OnValueChanged -= OnVelocityValueChanged;
        }

        private void OnVelocityValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            _avatarAnimator.SetFloat(VelocityXHash, newValue.x);
            _avatarAnimator.SetFloat(VelocityZHash, newValue.y);
        }

        [ClientRpc]
        public void PlayDamageAnimationClientRpc()
        {
            _avatarAnimator.SetInteger(ActionHash, 2);
            _avatarAnimator.SetTrigger(TriggerHash);
        }

        /// <summary>
        /// 0 for dash forward
        /// 1 for dash backward
        /// 2 for dash left
        /// 3 for dash right
        /// </summary>
        /// <param name="index"></param>
        [ClientRpc]
        public void PlayDashAnimationClientRpc(int index)
        {
            _avatarAnimator.SetInteger(ActionHash, 1);
            _avatarAnimator.SetInteger(TriggerNumberHash, index);
            _avatarAnimator.SetTrigger(TriggerHash);
        }

        /// <summary>
        /// 0 for single basic attack
        /// 1 for basic attack in a row (3)
        /// 2 for special attack 1
        /// 3 for special attack 2
        /// 4 for move attack
        /// </summary>
        /// <param name="index"></param>
        [ClientRpc]
        public void PlayAttackAnimationClientRpc(int index)
        {
            _avatarAnimator.SetInteger(ActionHash, 0);
            _avatarAnimator.SetInteger(TriggerNumberHash, index);
            _avatarAnimator.SetTrigger(TriggerHash);
        }

        #region Animation Event Receivers
        public void AnimationEventReceiver_FootL()
        {
            // TODO: Footstep sound
        }

        public void AnimationEventReceiver_FootR()
        {
            // TODO: Footstep sound
        }

        public void AnimationEventReceiver_Land()
        {
            // TODO: Dash landing sound
        }

        public void AnimationEventReceiver_Hit()
        {
            if (IsServer)
                CastSpell();
        }

        public void AnimationEventReceiver_Shoot()
        {

        }
        #endregion
    }
}
