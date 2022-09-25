using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;

namespace Holoi.Reality.MOFO
{
    public class MofoRealityManager : RealityManager
    {
        

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsServer)
            {
                // TODO: Update host hand position
            }
        }
    }
}