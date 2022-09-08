using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App.Sample
{
    public class SampleRealityMenuController : MonoBehaviour
    {
        public void EnterReality()
        {
            HoloKitApp.Instance.EnterRealityAsHost();
        }

        public void JoinReality()
        {
            HoloKitApp.Instance.JoinRealityAsSpectator();
        }
    }
}
