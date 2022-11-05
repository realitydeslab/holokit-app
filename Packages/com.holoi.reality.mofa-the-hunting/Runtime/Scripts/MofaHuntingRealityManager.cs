using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunt
{
    public class MofaHuntingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Hunt")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        protected override void Start()
        {
            base.Start();

            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
            }
        }
    }
}
