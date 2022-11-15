using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using MalbersAnimations.Utilities;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonController : NetworkBehaviour
    {
        [SerializeField] private Aim _aim;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _aim.MainCamera = HoloKitCamera.Instance.CenterEyePose;
        }
    }
}
