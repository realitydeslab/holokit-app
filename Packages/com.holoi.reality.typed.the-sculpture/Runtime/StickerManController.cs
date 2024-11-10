// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typed.TheSculpture
{
    public class StickerManController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _vfx;

        private void Start()
        {
            ARObjectAdjuster.Instance.SetARObject(transform);
        }

        private void Update()
        {
            var hostHandPose = ((TypedTheSculptureRealityManager) HoloKitApp.Instance.RealityManager).HostHandPose;

            if (hostHandPose.IsActive)
            {
                _vfx.SetBool("IsInteractable", true);
                _vfx.SetVector3("Hand", hostHandPose.transform.position);
            }
            else
            {
                _vfx.SetBool("IsInteractable", false);
            }
        }
    }
}
