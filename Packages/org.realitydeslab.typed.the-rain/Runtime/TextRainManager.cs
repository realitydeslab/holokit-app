// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using UnityEngine.VFX;
using RealityDesignLab.Library.HoloKitApp;
using RealityDesignLab.Library.ARUX;

namespace RealityDesignLab.Typed.TheRain
{
    public class TextRainManager : RealityManager
    {
        
        Transform _head;
        Transform _rightHand;
        Transform _leftHand;

        BoneController _bone;

        VisualEffect _vfxCloud;

        [Header("vfx rain")]
        VisualEffect _vfxRain;

        bool _isValid = true;

        void Start()
        {
            _vfxCloud = GetComponent<VisualEffect>();
            _vfxRain = transform.GetChild(0).GetComponent<VisualEffect>();
        }

        void Update()
        {

            if (_isValid)
            {
                UpdateVisual();
            }
        }

        void UpdateVisual()
        {
            //if (IsServer)
            //{
            //    var serverEye = FindObjectOfType<RainTypoRealityManager>().ServerCenterEye;

            //    _vfxRain.SetVector3("Head Position_position", serverEye.position - (serverEye.up * 0.15f));

            //    var estimateChestPos = serverEye.position - (serverEye.up * 0.5f);

            //    _vfxRain.SetVector3("Chest Position_position", estimateChestPos);

            //    var hitPos = FindObjectOfType<RainTypoRealityManager>().HitPosition;

            //    _vfxRain.SetVector3("Plane Position_position", hitPos);

            //    UpdateVisualClientRpc(serverEye.position, estimateChestPos, hitPos);
            //}

        }

        [ClientRpc] void UpdateVisualClientRpc(Vector3 eyePos, Vector3 chestPos, Vector3 hitPos)
        {
            if (IsServer)
            {
                return;
            }

            _vfxRain.SetVector3("Head Position_position", eyePos);

            _vfxRain.SetVector3("Chest Position_position", chestPos);

            _vfxRain.SetVector3("Plane Position_position", hitPos);
        }
    }
}
