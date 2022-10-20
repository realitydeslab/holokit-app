using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasController : MonoBehaviour
    {
        public VisualEffect vfx;
        public Animator vfxAnimator;
        public Animator setaAnimator;

        [SerializeField] HoverableObject _hoverableObject;

        HandObject _ho;

        void Start()
        {
            _ho = HandObject.Instance;

            _hoverableObject.OnLoadedEvents.AddListener(FindObjectOfType<QuantumBuddhasSceneManager>().SwitchToNextVFXNetWork);
        }

        void Update()
        {
            vfx.SetVector3("Hand Position", _ho.transform.position);
        }
    }
}
