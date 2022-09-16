using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Mofa.Base
{
    public class SpellMovementController : NetworkBehaviour
    {
        public float Speed;

        private void Start()
        {
            GetComponent<Rigidbody>().velocity = Speed * transform.forward;
        }
    }
}