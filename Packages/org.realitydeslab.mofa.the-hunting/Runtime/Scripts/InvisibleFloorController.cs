// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class InvisibleFloorController : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetInvisibleFloor(gameObject);
            //if (HoloKitUtils.IsRuntime)
            //{
            //    GetComponentInChildren<MeshRenderer>().enabled = false;
            //}
        }
    }
}
