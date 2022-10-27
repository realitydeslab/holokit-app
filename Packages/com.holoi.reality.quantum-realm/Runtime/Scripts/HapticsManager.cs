using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.CoreHaptics;
using HoloKit;

namespace Holoi.Reality.QuantumRealm
{
    public class HapticsManager : MonoBehaviour
    {

        public CHHapticEngine HapticsEngine;


        public void Awake()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HapticsEngine = new CHHapticEngine { IsAutoShutdownEnabled = false };
                HapticsEngine.Start();
            }

        }

        private void Start()
        {
            
        }
    }
}
