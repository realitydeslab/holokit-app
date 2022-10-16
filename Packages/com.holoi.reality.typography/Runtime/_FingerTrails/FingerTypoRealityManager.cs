using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.ARUX;

namespace Holoi.Reality.TypoGraphy
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
                //HandObject.Instance.enabled = true;
                //ARRayCastController.Instance.enabled = true;
            }
            else
            {
                HoloKitHandTracker.Instance.enabled = false;
                //HandObject.Instance.enabled = false;
                //ARRayCastController.Instance.enabled = false;
            }

            //if (HoloKitApp.Instance.IsHost)
            //{

            //}
            //else
            //{
            //    for (int i = 0; i < _smoothTips.childCount; i++)
            //    {
            //        _smoothTips.GetChild(i).GetComponent<FollowMovementManager>().enabled = false;
            //    }
            //    for (int i = 0; i < _smoothSeconds.childCount; i++)
            //    {
            //        _smoothSeconds.GetChild(i).GetComponent<FollowMovementManager>().enabled = false;
            //    }
            //}
        }

        private void Update()
        {

        }

    }
}
