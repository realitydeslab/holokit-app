using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Reality.MOFO
{
    public class MofoRealityManager : RealityManager
    {
        [SerializeField] private NetworkObject _networkHandPrefab;

        private NetworkObject _networkHand;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _networkHand = Instantiate(_networkHandPrefab);
                _networkHand.Spawn();
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsServer)
            {
                if (HoloKitHandTracker.Instance.Valid)
                {
                    //_networkHand.transform.position = HoloKitHandTracker.Instance.GetHandJointPosition(HandJoint.Index3);
                }
            }
        }
    }
}