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

        //[SerializeField] bool _isSourceEnabled = false;
        [SerializeField] bool _isSourcePlaying = false;

        private void Awake()
        {
            //if(_isSourceEnabled)
            //{
            //    foreach (var source in _phaseSources)
            //    {
            //        source.Play();
            //    }
            //}
            //if (_isSourceEnabled)
            //{
            //    foreach (var source in _phaseSources)
            //    {
            //        source.Stop();
            //    }
            //}
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
    }
}
