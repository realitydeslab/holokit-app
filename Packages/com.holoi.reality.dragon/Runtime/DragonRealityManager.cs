// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Reality.Dragon
{
    public class DragonRealityManager : RealityManager
    {
        [SerializeField] private NetworkObject _networkHandPrefab;

        private NetworkObject _networkHand;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (_networkHandPrefab)
            {
                if (IsServer)
                {
                    _networkHand = Instantiate(_networkHandPrefab);
                    _networkHand.Spawn();
                }
            }
            else
            {

            }

        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (HoloKitHandTracker.Instance.IsValid)
                {
                    //_networkHand.transform.position = HoloKitHandTracker.Instance.GetHandJointPosition(HandJoint.Index3);
                }
            }
        }
    }
}