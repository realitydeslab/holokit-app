using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;
using HoloKit;

namespace Holoi.Reality.QuantumBuddhas
{
    public class PhaseManager : MonoBehaviour
    {
        public PHASESource _bg;
        public PHASEListener _pl;

        public void OpenPhase()
        {
            _bg.enabled = true;
            _pl.enabled = true;
        }

        public void DisablePhase()
        {

        }
    }
}
