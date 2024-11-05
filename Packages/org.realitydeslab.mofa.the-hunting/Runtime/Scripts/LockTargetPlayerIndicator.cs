// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using org.realitydeslab.MOFABase;

namespace RealityDesignLab.MOFA.TheHunting
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
