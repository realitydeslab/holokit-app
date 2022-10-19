using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;
using HoloKit;

namespace Holoi.Reality.QuantumRealm
{
    public class PhaseManager : MonoBehaviour
    {
        public PHASESource _phaseSources;
        public PHASEListener _phaseListener;

        private void Awake()
        {

        }

        public void PlayPhaseSource()
        {
            if (!_phaseListener.isActiveAndEnabled)
            {
                Debug.Log("Enable Phase Listener");
                _phaseListener.enabled = true;
            }

            if (!_phaseSources.IsPlaying())
            {
                Debug.Log("play Phase Source");

                _phaseSources.Play();

            }
            else
            {
                Debug.Log("stop Phase Source");

                _phaseSources.Stop();
            }
        }

        // now PHASE has a bug that will not stop playing after exited scene. So for now we need to handle it manully;
        public void OnSceneExited()
        {
            _phaseSources.Stop();
            _phaseSources.DestroyFromPHASE();
        }
    }
}
