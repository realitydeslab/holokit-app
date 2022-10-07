using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using HoloKit;

namespace Holoi.Reality.TypoGraphy
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
                // trial vfx
                _vfxs[i].gameObject.transform.position = _softTips[i].position;
                _vfxs[i].SetVector3("Tip Normal", _HGM.TipNormals[i]);

                // particel vfx
                //_vfxs[i].gameObject.transform.position = _softTips[i].position;
                //_vfxs[i].SetVector3("Tip Normal", _HGM.TipNormals[i]);
                //_vfxs[i].SetVector3("V Direction", _HGM.TipVelocityDirection[i]);
                //Debug.Log(_vfxs[i].GetVector3("V Direction"));
            }
        }
        public void HideJoint()
        {
            if (_HHT.Visible)
            {
                _HHT.Visible = false;

            }
            else
            {
                _HHT.Visible = true;

            }
        }
    }
}
