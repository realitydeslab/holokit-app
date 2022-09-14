using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Holoi.Mofa.Base
{
    public class SpellLifetimeController : NetworkBehaviour
    {
        public float Lifetime;

        public float DestroyDelay;

        [SerializeField] private AudioClip _spawnSound;

        private float _spawnTime;

        public event Action OnSpawned;

        public event Action OnDead;

        public override void OnNetworkSpawn()
        {
            _spawnTime = NetworkManager.ServerTime.TimeAsFloat;
            OnSpawned?.Invoke();
            PlaySpawnSound();
        }

        private void PlaySpawnSound()
        {
            if (_spawnSound != null)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = _spawnSound;
                audioSource.Play();
            }
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (NetworkManager.ServerTime.TimeAsFloat - _spawnTime > Lifetime)
                {
                    OnDeadClientRpc();
                    Destroy(this, DestroyDelay);
                }
            }
        }

        [ClientRpc]
        private void OnDeadClientRpc()
        {
            OnDead?.Invoke();
        }
    }
}