using System;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    public class SpellLifetimeController : NetworkBehaviour
    {
        [SerializeField] private float _lifetime;

        [SerializeField] private AudioClip _spawnSound;

        [SerializeField] private float _destroyDelay;

        private float _duration;

        public event Action OnLifetimeEnded;

        private void OnEnable()
        {
            PlaySpawnSound();
            _duration = 0f;
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _duration += Time.fixedDeltaTime;
                if (_duration > _lifetime)
                {
                    OnLifetimeEndedClientRpc();
                    GetComponent<NetworkObject>().Despawn();
                }
            }
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

        [ClientRpc]
        private void OnLifetimeEndedClientRpc()
        {
            Debug.Log($"{gameObject.name}: OnLifetimeEndedClientRpc");
            OnLifetimeEnded?.Invoke();
        }
    }
}