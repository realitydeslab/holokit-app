using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;

namespace Holoi.Reality.QuantumBuddhas
{
    public class BuddhasController : MonoBehaviour
    {
        public VisualEffect vfx;

        [SerializeField] HoverableObject _hoverableObject;

        HandObject _ho;

        void Start()
        {
            _ho = HandObject.Instance;

            _hoverableObject.OnLoadedEvents.AddListener(FindObjectOfType<QuantumBuddhasSceneManager>().SwitchToNextVFX);
        }

        void Update()
        {
            vfx.SetVector3("Hand Position", _ho.transform.position);
        }
    }
}
