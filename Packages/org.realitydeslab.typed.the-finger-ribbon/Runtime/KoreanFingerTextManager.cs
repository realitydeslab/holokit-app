// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace RealityDesignLab.Typed.TheFingerRibbon
{
    public class KoreanFingerTextManager : MonoBehaviour
    {
        [SerializeField] HoloKitHandTracker _HHT;
        [SerializeField] GameObject[] _vfxs = new GameObject[5];
        [SerializeField] Transform[] _softTips = new Transform[5];

        void Start()
        {

        }

        void FixedUpdate()
        {

            for (int i = 0; i < 5; i++)
            {
                // particel vfx
                _vfxs[i].transform.position = _softTips[i].position;

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
