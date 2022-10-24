using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.Typography
{
    public class StickerManController : MonoBehaviour
    {
        HandObject _ho;
        [SerializeField] VisualEffect _vfx;

        // Start is called before the first frame update
        void Start()
        {
            _ho = FindObjectOfType<HandObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_ho.IsValid)
            {
                _vfx.SetFloat("Rate", 1);
            }
            else
            {
                _vfx.SetFloat("Rate", 0);
            }
            _vfx.SetVector3("Hand", _ho.transform.position);
        }
    }
}