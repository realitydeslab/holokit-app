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
        public PHASESoundEventNodeGraph _phaseSoundEventNodeGraph;

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
                Debug.Log("play Phase Source");

                _phaseSource.Play();

            }
        }

        // now PHASE has a bug that will not stop playing after exited scene. So for now we need to handle it manully;
        public void OnSceneExited()
        {
            Helpers.PHASEStop();

            Debug.Log("OnSceneExited");

            //if(_phaseSource != null)
            //{
            //    Debug.Log("_phaseSources");

            //    _phaseSource.Stop();
            //    _phaseSource.DestroyFromPHASE();
            //    _phaseSource.enabled = false;
            //    Destroy(_phaseSource);
            //}
            //if (_phaseListener != null)
            //{
            //    Debug.Log("_phaseListener");
            //    _phaseListener.enabled = false;
                
            //    Destroy(_phaseListener);
            //}
        }

        private void OnDestroy()
        {
            OnSceneExited();
        }
    }
}
