using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheGhost
{
    public class DetectionWave : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;


        }
    }
}
