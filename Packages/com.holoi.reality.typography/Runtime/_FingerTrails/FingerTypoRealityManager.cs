using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typography
{
    public class FingerTypoRealityManager : RealityManager
    {
        [Header("Reality Objects")]
        public Transform _smoothTips;
        public Transform _smoothSeconds;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.Instance.enabled = true;
            }
            else
            {
                HoloKitHandTracker.Instance.enabled = false;
            }
        }

        private void Update()
        {

        }

    }
}
