using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.CoreHaptics;

namespace Holoi.Reality.QuantumRealm
{
    public class HapticsManager : MonoBehaviour
    {

        public CHHapticEngine HapticsEngine;


        public void Awake()
        {
            HapticsEngine = new CHHapticEngine { IsAutoShutdownEnabled = false };
            HapticsEngine.Start();
        }
    }
}
