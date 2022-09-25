using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using System;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    public class AttackSpell : NetworkBehaviour
    {
        public bool HitOnce = true;

        [SerializeField] private AudioClip _hitSound;

        [SerializeField] private float _destroyDelay;

        public event Action OnHit;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                var mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                MofaTeam attackerTeam = mofaRealityManager.Players[OwnerClientId].Team.Value;
                MofaTeam victimTeam = mofaRealityManager.Players[other.GetComponentInParent<NetworkObject>().OwnerClientId].Team.Value;
                if (attackerTeam != victimTeam)
                {
                    damageable.OnDamaged(OwnerClientId);
                    OnHitClientRpc();
                    if (HitOnce)
                    {
                        GetComponent<Collider>().enabled = false;
                        Destroy(gameObject, _destroyDelay);
                    }
                }
            }
        }

        [ClientRpc]
        private void OnHitClientRpc()
        {
            if (HitOnce)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }
            OnHit?.Invoke();
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            if (_hitSound != null)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = _hitSound;
                audioSource.Play();
            }
        }
    }
}