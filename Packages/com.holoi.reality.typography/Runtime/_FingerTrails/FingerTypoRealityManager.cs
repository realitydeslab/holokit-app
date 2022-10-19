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
        [Header("Server Controlled Objects")]
        [SerializeField] List<Transform> _softTips;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.Instance.enabled = true;
                foreach (var tip in _softTips)
                {
                    tip.GetComponent<FollowMovementManager>().enabled = true;
                }
            }
            else
            {
                HoloKitHandTracker.Instance.enabled = false;
                foreach (var tip in _softTips)
                {
                    tip.GetComponent<FollowMovementManager>().enabled = false;
                }
            }
        }

        private void Update()
        {

        }

    }
}
