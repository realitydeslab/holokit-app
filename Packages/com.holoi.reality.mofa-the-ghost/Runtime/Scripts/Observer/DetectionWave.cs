using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheGhost
{
    public class DetectionWave : NetworkBehaviour
    {
        [SerializeField] private float _initialForce = 100f;

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
