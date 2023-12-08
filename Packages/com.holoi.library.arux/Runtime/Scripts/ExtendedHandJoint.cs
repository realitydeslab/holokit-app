// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class ExtendedHandJoint : MonoBehaviour
    {
        [SerializeField] private HoloKitHandJoint _handJoint;

        [SerializeField] private float _extendedLength;

        private void Update()
        {
            if (HoloKitHandTracker.Instance.IsValid)
            {
                Vector3 handJointPosition = HoloKitHandTracker.Instance.GetHandJointPosition(_handJoint);
                Vector3 direction = (handJointPosition - HoloKitCamera.Instance.CenterEyePose.position).normalized;
                transform.position = handJointPosition + _extendedLength * direction;
            }
        }
    }
}