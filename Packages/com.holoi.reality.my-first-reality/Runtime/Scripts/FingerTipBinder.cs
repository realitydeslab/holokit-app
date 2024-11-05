// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Reality.MyFirstReality
{
    public class FingerTipBinder : MonoBehaviour
    {
        private void Start() {
            Debug.Log("FingerTip");
        }
        private void Update()
        {
            transform.position = HoloKitHandTracker.Instance.GetHandJointPosition(HandJoint.IndexTip);
        }
    }
}
