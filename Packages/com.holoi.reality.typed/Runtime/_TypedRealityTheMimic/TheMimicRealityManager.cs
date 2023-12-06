using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class TheMimicRealityManager : RealityManager
    {
        void Start()
        {
            HoloKit.HoloKitHandTracker.Instance.IsActive = true;
        }

        void Update()
        {

        }
    }
}