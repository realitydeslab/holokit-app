using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheDucks
{
    public class Duck : NetworkBehaviour
    {
        private float _lifetimeElapsed = 0f;

        private const float InitialForce = 100f;

        private const float Lifetime = 4f;

        public override void OnNetworkSpawn()
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(InitialForce * transform.forward);
        }

        private void Update()
        {
            if (IsServer)
            {
                _lifetimeElapsed += Time.deltaTime;
                if (_lifetimeElapsed > Lifetime)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
