// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFA.TheHunting
{
    public class LockTargetPlayerIndicator : MonoBehaviour
    {
        public Vector3 Offset = new(0f, 0.2f, 0f);

        public MofaPlayer MofaPlayer;

        private void LateUpdate()
        {
            // The pose visualizer should always look at the local camera.
            transform.LookAt(HoloKit.HoloKitCameraManager.Instance.CenterEyePose);
            if (MofaPlayer != null)
            {
                transform.position = MofaPlayer.transform.position + Offset;
            }
        }
    }
}
