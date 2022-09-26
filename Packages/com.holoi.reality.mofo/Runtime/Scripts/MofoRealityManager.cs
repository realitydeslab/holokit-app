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
        public NetworkVariable<Vector3> NetworkHandPosition = new(Vector3.zero, NetworkVariableReadPermission.Everyone);

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsServer)
            {

            }
        }
    }
}