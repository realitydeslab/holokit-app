// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUICameraForwardVfxFeeder : MonoBehaviour
    {
        private VisualEffect _vfx;

        private void Start()
        {
            _vfx = GetComponent<VisualEffect>();
        }

        private void Update()
        {
            _vfx.SetVector3("CameraForward", Camera.main.transform.forward);
        }
    }
}
