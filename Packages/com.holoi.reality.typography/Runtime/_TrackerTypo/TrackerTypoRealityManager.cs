using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;


namespace Holoi.Reality.Typography
{
    public class TrackerTypoRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _serverCenterEye;

        Transform _centerEye;

        [HideInInspector] public Vector3 HitPosition = Vector3.down;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

        }

        private void Start()
        {
            _centerEye = HoloKitCamera.Instance.CenterEyePose;

             InitServerCenterEye();

        }

        void Update()
        {


        }

        void InitServerCenterEye()
        {

            if (HoloKitApp.Instance.IsHost)
            {
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = true;
            }
            else
            {
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = false;

            }
        }
    }
}