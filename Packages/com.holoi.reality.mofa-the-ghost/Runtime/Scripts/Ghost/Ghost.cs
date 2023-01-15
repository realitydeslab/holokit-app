using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Reality.MOFATheGhost
{
    public class Ghost : NetworkBehaviour
    {
        private CharacterController _characterController;

        private float _movementSpeed = 0.01f;

        private void Awake()
        {
            UI.MofaGhostJoystickController.OnAxisChanged += OnAxisChanged;

            _characterController = GetComponent<CharacterController>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UI.MofaGhostJoystickController.OnAxisChanged -= OnAxisChanged;
        }

        /// <summary>
        /// Called when the ghost player is inputting.
        /// </summary>
        /// <param name="axis"></param>
        private void OnAxisChanged(Vector2 axis)
        {
            Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
            Vector3 horizontalForward = Vector3.ProjectOnPlane(centerEyePose.forward, Vector3.up);
            Vector3 horizontalRight = Vector3.ProjectOnPlane(centerEyePose.right, Vector3.up);

            Vector3 motion = _movementSpeed * (axis.y * horizontalForward + axis.x * horizontalRight);
            // Make the ghost heading to the movement direction
            transform.rotation = Quaternion.LookRotation(motion.normalized);
            // Move the ghost
            _characterController.Move(motion);
        }

        /// <summary>
        /// This method is called by the detection wave when the ghost is detected.
        /// </summary>
        [ClientRpc]
        public void OnDetectedClientRpc()
        {

        }
    }
}
