using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using HoloKit;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class FingerTextManager : MonoBehaviour
    {
        [SerializeField] HoloKitHandTracker _HHT;
        [SerializeField] HandGestureManager _HGM;
        [SerializeField] VisualEffect[] _vfxs = new VisualEffect[5];
        [SerializeField] Transform[] _softTips;

        void Start()
        {

        }

        void FixedUpdate()
        {

                for (int i = 0; i < 5; i++)
                {
                    // particel vfx
                    _vfxs[i].gameObject.transform.position = _softTips[i].position;
                    _vfxs[i].SetVector3("Tip Normal", _HGM.TipNormals[i]);
                    _vfxs[i].SetVector3("V Direction", _HGM.TipVelocityDirection[i]);
                }

        }
        public void HideJoint()
        {
            if (_HHT.IsVisible)
            {
                _HHT.IsVisible = false;

            }
            else
            {
                _HHT.IsVisible = true;

            }
        }
    }
}
