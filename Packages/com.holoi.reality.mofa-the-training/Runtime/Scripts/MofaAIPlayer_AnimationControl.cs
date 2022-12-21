using System.Collections;
using System.Collections.Generic;
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

        public int ActionHash;

        public int TriggerNumberHash;

        public int TriggerHash;

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
        public void PlayDamageClientRpc()
        {
            _avatarAnimator.SetInteger(ActionHash, 2);
            _avatarAnimator.SetTrigger(TriggerHash);
        }

        #region Animation Event Receivers
        public void AnimationEventReceiver_FootL()
        {

        }

        public void AnimationEventReceiver_FootR()
        {

        }

        public void AnimationEventReceiver_Land()
        {

        }

        public void AnimationEventReceiver_Hit()
        {

        }

        public void AnimationEventReceiver_Shoot()
        {

        }
        #endregion
    }
}
