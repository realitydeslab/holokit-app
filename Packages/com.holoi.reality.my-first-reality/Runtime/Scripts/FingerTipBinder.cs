// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
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
