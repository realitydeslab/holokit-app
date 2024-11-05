// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RealityDesignLab.Reality.QuantumRealm
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
