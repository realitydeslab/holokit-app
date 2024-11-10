// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    public class ShieldSpell : NetworkBehaviour, IDamageable
    {
        // How many times this shield can block attack
        [SerializeField] private int _maxHealth;

        [SerializeField] private AudioClip _beingHitSound;

        [SerializeField] private AudioClip _beingDestroyedSound;

        [SerializeField] private float _destroyDelay;

        private readonly NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private AudioSource _audioSource;

        public event Action OnBeingHit;

        public event Action OnBeingDestroyed;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _currentHealth.Value = _maxHealth;
                GetComponent<Collider>().enabled = true;
            }
            else
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        // Host only
        public void OnDamaged(ulong attackerClientId)
        {
            _currentHealth.Value--;
            if (_currentHealth.Value > 0)
            {
                OnBeingHitClientRpc();
            }
            else
            {
                OnBeingDestroyedClientRpc();
                GetComponent<Collider>().enabled = false;
                StartCoroutine(HoloKitAppUtils.WaitAndDo(_destroyDelay, () =>
                {
                    GetComponent<NetworkObject>().Despawn();
                }));
            }
        }

        [ClientRpc]
        private void OnBeingHitClientRpc()
        {
            OnBeingHit?.Invoke();
            PlayBeingHitSound();
        }

        [ClientRpc]
        private void OnBeingDestroyedClientRpc()
        {
            OnBeingDestroyed?.Invoke();
            PlayBeingDestroyedSound();
        }

        private void PlayBeingHitSound()
        {
            if (_beingHitSound != null)
            {
                _audioSource.clip = _beingHitSound;
                _audioSource.Play();
            }
        }

        private void PlayBeingDestroyedSound()
        {
            if (_beingDestroyedSound != null)
            {
                _audioSource.clip = _beingDestroyedSound;
                _audioSource.Play();
            }
        }
    }
}
