using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class PhaseManager : MonoBehaviour
    {
        public PHASESource _phaseSource;
        public PHASEListener _phaseListener;

        private void Awake()
        {

        }
        private void Start()
        {

        }
        public void PlayPhaseSource()
        {
            if (!_phaseSource.IsPlaying())
            {
                _phaseSource.Play();
            }
        }

        public void StopPhaseSource()
        {
            if (_phaseSource.IsPlaying())
            {
                _phaseSource.Stop();
            }
        }

        // now PHASE has a bug that will not stop playing after exited scene. So for now we need to handle it manully;
        public void OnSceneExited()
        {
            Helpers.PHASEStop();
        }

        private void OnDestroy()
        {
            OnSceneExited();
        }
    }
}
