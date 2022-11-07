using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using Unity.Netcode;
using HoloKit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Holoi.Reality.Typography
{
    public class BodyTrackingRealityManager : RealityManager
    {
        [Header("AR Base Objects")]
        [SerializeField] Transform _centerEye;

        public bool IsBodyValid = false;
        bool _isFirstTimeFindBody = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Start()
        {
            if(_centerEye == null) _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
            if (FindObjectOfType<BoneController>() != null)
            {
                if (!_isFirstTimeFindBody)
                {
                    IsBodyValid = true;
                    //_phaseManager.PlayPhaseSource();
                    _isFirstTimeFindBody = true;
                }
                else
                {
                    IsBodyValid = true;
                }

            }
            else
            {
                Debug.Log("not found a body");
                IsBodyValid = false;
                //_phaseManager.StopPhaseSource();
            }
        }
    }
}