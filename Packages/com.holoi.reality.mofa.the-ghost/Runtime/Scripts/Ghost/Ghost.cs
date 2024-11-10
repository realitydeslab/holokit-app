// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFA.TheGhost
{
    public class Ghost : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 20;

        [SerializeField] private SkinnedMeshRenderer _ghostRenderer;

        [SerializeField] private SkinnedMeshRenderer _weaponRenderer;

        public NetworkVariable<int> CurrentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private CharacterController _characterController;

        private Animator _animator;

        private float _movementSpeed = 3f;

        private float _lastAttackTime;

        private const float AttackCD = 8f;

        private void Start()
        {
            UI.MofaGhostJoystickController.OnAxisChanged += OnAxisChanged;
            UI.MofaGhostJoystickController.OnInputStarted += OnInputStartedClientRpc;
            UI.MofaGhostJoystickController.OnInputStopped += OnInputStoppedClientRpc;
            if (HoloKitApp.Instance.IsPuppeteer)
                UI.GhostPlayerUIPanel.OnTriggered += OnUITriggered;

            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                CurrentHealth.Value = _maxHealth;
            }
            else
            {
                ChangeToInvisible();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UI.MofaGhostJoystickController.OnAxisChanged -= OnAxisChanged;
            UI.MofaGhostJoystickController.OnInputStarted -= OnInputStartedClientRpc;
            UI.MofaGhostJoystickController.OnInputStopped -= OnInputStoppedClientRpc;
            if (HoloKitApp.Instance.IsPuppeteer)
                UI.GhostPlayerUIPanel.OnTriggered -= OnUITriggered;
        }

        /// <summary>
        /// Called when the ghost player is inputting.
        /// </summary>
        /// <param name="axis"></param>
        private void OnAxisChanged(Vector2 axis)
        {
            Transform centerEyePose = HoloKitCameraManager.Instance.CenterEyePose;
            Vector3 horizontalForward = Vector3.ProjectOnPlane(centerEyePose.forward, Vector3.up);
            Vector3 horizontalRight = Vector3.ProjectOnPlane(centerEyePose.right, Vector3.up);

            Vector3 motion = _movementSpeed * Time.deltaTime * (axis.y * horizontalForward + axis.x * horizontalRight);
            // Make the ghost heading to the movement direction
            transform.rotation = Quaternion.LookRotation(motion.normalized);
            // Move the ghost
            _characterController.Move(motion);
        }

        [ClientRpc]
        private void OnInputStartedClientRpc()
        {
            _animator.SetFloat("Velocity", 1);
        }

        [ClientRpc]
        private void OnInputStoppedClientRpc()
        {
            _animator.SetFloat("Velocity", 0);
        }

        /// <summary>
        /// This method is called by the detection wave when the ghost is detected.
        /// </summary>
        [ClientRpc]
        public void OnDetectedClientRpc()
        {
            Debug.Log("[Ghost] On detected");
            if (HoloKitApp.Instance.IsPuppeteer)
            {
                // TODO: The puppeteer should know its ghost has been detected
            }
            else
            {
                // Both the observer and the attacker should see the ghost for a period of time
                OnBeingRevealed();
            }
        }

        private void OnBeingRevealed()
        {
            if (!IsServer)
                ChangeToVisible();
        }

        public void OnDamaged(ulong attackerClientId)
        {
            OnDamagedClientRpc();

            CurrentHealth.Value--;
            if (CurrentHealth.Value == 0)
            {
                OnGhostDeadClientRpc();
            }
        }

        [ClientRpc]
        private void OnDamagedClientRpc()
        {
            Debug.Log("[Ghost] On damaged");
            OnBeingRevealed();
            OnBeingHit();
        }

        [ClientRpc]
        private void OnGhostDeadClientRpc()
        {
            Debug.Log("Ghost is dead");
            _animator.SetTrigger("OnDeath");
        }

        private void OnBeingHit()
        {
            _animator.SetTrigger("OnHit");
        }

        private void ChangeToVisible()
        {
            var ghostMaterial = _ghostRenderer.material;
            var weaponMaterial = _weaponRenderer.material;
            LeanTween.value(0f, 1f, 1f)
                .setOnUpdate((float t) =>
                {
                    ghostMaterial.SetFloat("_Alpha", t);
                    weaponMaterial.SetFloat("_Alpha", t);
                })
                .setOnComplete(() =>
                {
                    ChangeToInvisible();
                });
        }

        private void ChangeToInvisible()
        {
            var ghostMaterial = _ghostRenderer.material;
            var weaponMaterial = _weaponRenderer.material;
            LeanTween.value(1f, 0f, 1f)
                .setOnUpdate((float t) =>
                {
                    ghostMaterial.SetFloat("_Alpha", t);
                    weaponMaterial.SetFloat("_Alpha", t);
                });
        }

        private void OnUITriggered()
        {
            if (Time.time - _lastAttackTime > AttackCD)
            {
                _lastAttackTime = Time.time;
                OnAttackClientRpc();
            }
            
        }

        [ClientRpc]
        private void OnAttackClientRpc()
        {
            _animator.SetTrigger("Attack");
        }
    }
}
