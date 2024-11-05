// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using HoloKit;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.Typed.TheFingerRibbon
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
