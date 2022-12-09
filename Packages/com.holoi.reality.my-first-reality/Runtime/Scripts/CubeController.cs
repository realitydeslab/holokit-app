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
            if (IsSpawned && !IsServer)
            {
                transform.position += new Vector3(0.001f, 0f, 0f);
            }
        }
    }
}
