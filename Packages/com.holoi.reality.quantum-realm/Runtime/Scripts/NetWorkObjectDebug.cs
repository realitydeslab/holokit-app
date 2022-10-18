using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.QuantumRealm
{
    public class NetWorkObjectDebug : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log(this.gameObject.name + ": connected");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
