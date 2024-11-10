// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class ExtendedHandJoint : MonoBehaviour
    {
        [SerializeField] private HandJoint _handJoint;

        [SerializeField] private float _extendedLength;

        private void Update()
        {
            if (HoloKitHandTracker.Instance.IsValid)
            {
                Vector3 handJointPosition = HoloKitHandTracker.Instance.GetHandJointPosition(_handJoint);
                Vector3 direction = (handJointPosition - HoloKitCameraManager.Instance.CenterEyePose.position).normalized;
                transform.position = handJointPosition + _extendedLength * direction;
            }
        }
    }
}