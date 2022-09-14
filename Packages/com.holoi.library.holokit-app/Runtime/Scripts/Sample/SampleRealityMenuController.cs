using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.Sample
{
    public class SampleRealityMenuController : MonoBehaviour
    {
        [SerializeField] private Reality _sampleReality;

        private void Start()
        {
            HoloKitApp.Instance.CurrentReality = _sampleReality;
            HoloKitApp.Instance.InitializeNetworkManager();
        }

        public void EnterReality()
        {
            //HoloKitApp.Instance.EnterRealityAsHost();
        }

        public void JoinReality()
        {
            //HoloKitApp.Instance.JoinRealityAsSpectator();
        }
    }
}
