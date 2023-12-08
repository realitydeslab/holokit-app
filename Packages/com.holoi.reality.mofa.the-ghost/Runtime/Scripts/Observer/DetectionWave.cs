// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFA.TheGhost
{
    public class DetectionWave : NetworkBehaviour
    {
        [SerializeField] private float _initialForce = 100f;

        [SerializeField] private float _lifetime = 6f;

        private float _accumulatedLifetime;

        private void Update()
        {
            if (IsServer)
            {
                _accumulatedLifetime += Time.deltaTime;
                if (_accumulatedLifetime > _lifetime)
                {
                    Destroy(gameObject);
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            GetComponent<Rigidbody>().AddForce(_initialForce * transform.forward);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            if (other.TryGetComponent<Ghost>(out var ghost))
            {
                ghost.OnDetectedClientRpc();
            }
        }
    }
}
