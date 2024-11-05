// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
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
