using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MyFirstReality
{
    public class CubeController : NetworkBehaviour
    {
        private void Update()
        {
            if (IsSpawned && IsHost)
            {
                // Move the cube in sine wave
                transform.position = new Vector3(Mathf.Sin(Time.time), transform.position.y, transform.position.z);
            }
        }
    }
}
