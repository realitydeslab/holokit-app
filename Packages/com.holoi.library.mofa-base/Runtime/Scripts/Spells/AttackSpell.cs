using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;
using System;

namespace Holoi.Mofa.Base
{
    public class AttackSpell : NetworkBehaviour
    {
        public bool HitOnce = true;

        public float DestroyDelay;

        [SerializeField] private AudioClip _hitSound;

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
                var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                MofaTeam attackerTeam = mofaRealityManager.Players[OwnerClientId].Team.Value;
                MofaTeam victimTeam = mofaRealityManager.Players[other.GetComponentInParent<NetworkObject>().OwnerClientId].Team.Value;
                if (attackerTeam != victimTeam)
                {
                    damageable.OnHit();
                    OnHitClientRpc();
                    if (HitOnce)
                    {
                        Destroy(this, DestroyDelay);
                    }
                }
            }
        }

        [ClientRpc]
        private void OnHitClientRpc()
        {
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