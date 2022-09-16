using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Holoi.Mofa.Base
{
    public class ShieldSpell : NetworkBehaviour, IDamageable
    {
        // How many times this shield can block attack
        public int MaxHealth;

        [SerializeField] private float _destroyDelay;

        private NetworkVariable<int> _currentHealth;

        public event Action OnBeingHit;

        public event Action OnBeingDestroyed;

        private void Awake()
        {
            _currentHealth = new(MaxHealth, NetworkVariableReadPermission.Everyone);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        public void OnDamaged(ulong clientId)
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
        }

        [ClientRpc]
        private void OnBeingDestroyedClientRpc()
        {
            OnBeingDestroyed?.Invoke();
        }
    }
}