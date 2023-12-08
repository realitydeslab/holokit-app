// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

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
