// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.HoloKitApp.UI
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
