using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;
using Apple.PHASE;

namespace Holoi.Reality.Typography
{
    public class BuddhasController : MonoBehaviour
    {
        public VisualEffect vfx;

        [SerializeField] HoverableObject _hoverableObject;
        [SerializeField] PHASESource _phaseSource;

        HandObject _ho;

        void Start()
        {
            _ho = HandObject.Instance;

            _hoverableObject.OnTriggered.AddListener(FindObjectOfType<BuddhasRealityManager>().OnInteractionTriggered);
            _hoverableObject.OnTriggered.AddListener(PlayPhase);
        }

        void PlayPhase()
        {
            _phaseSource.Play();
        }

        void Update()
        {
            vfx.SetVector3("Hand Position", _ho.transform.position);
        }
    }
}
