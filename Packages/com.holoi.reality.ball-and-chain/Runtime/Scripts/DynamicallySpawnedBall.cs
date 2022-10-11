using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.BallAndChain
{
    public class DynamicallySpawnedBall : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log("[DynamicallySpawnedBall] OnNetworkSpawn");
        }
    }
}
