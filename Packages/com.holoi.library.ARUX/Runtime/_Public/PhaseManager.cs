using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;
using HoloKit;

namespace Holoi.Library.ARUX
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
            if (!_phaseSources.IsPlaying())
            {
                Debug.Log("play Phase Source");

                _phaseSources.Play();

            }
        }

        // now PHASE has a bug that will not stop playing after exited scene. So for now we need to handle it manully;
        public void OnSceneExited()
        {
            //_phaseSources.Stop();
            //_phaseSources.DestroyFromPHASE();

            Debug.Log("OnSceneExited");
            if(_phaseSources != null)
            {
                Debug.Log("_phaseSources");

                _phaseSources.Stop();
                _phaseSources.DestroyFromPHASE();
                _phaseSources.enabled = false;
                Destroy(_phaseSources);
            }
            if (_phaseListener != null)
            {
                Debug.Log("_phaseListener");

                //_phaseListener.enabled = false;
                //Destroy(_phaseListener);
            }
        }

        private void OnDestroy()
        {
            OnSceneExited();
        }
    }
}
