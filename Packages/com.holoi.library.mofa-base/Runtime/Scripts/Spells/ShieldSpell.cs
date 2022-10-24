using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    public class ShieldSpell : NetworkBehaviour, IDamageable
    {
        public bool OnHitDelegation => false;

        // How many times this shield can block attack
        public int MaxHealth;

        [SerializeField] private AudioClip _beingHitSound;

        [SerializeField] private AudioClip _beingDestroyedSound;

        [SerializeField] private float _destroyDelay;

        private NetworkVariable<int> _currentHealth;

        private AudioSource _audioSource;

        public event Action OnBeingHit;

        public event Action OnBeingDestroyed;

        private void Awake()
        {
            _currentHealth = new(MaxHealth, NetworkVariableReadPermission.Everyone);
            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        public void OnDamaged(Transform _, ulong attackerClientId)
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
                Destroy(gameObject, _destroyDelay);
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