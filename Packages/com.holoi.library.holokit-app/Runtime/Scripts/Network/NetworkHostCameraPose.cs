using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class NetworkHostCameraPose : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.RealityManager.SetNetworkHostCameraPose(this);
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                transform.SetPositionAndRotation(HoloKitCamera.Instance.CenterEyePose.position,
                                                 HoloKitCamera.Instance.CenterEyePose.rotation);
            }
        }
    }
}
