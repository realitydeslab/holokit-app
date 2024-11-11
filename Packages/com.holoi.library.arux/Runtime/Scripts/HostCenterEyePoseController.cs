// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Animations;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.ARUX
{
    public class HostCenterEyePoseController : MonoBehaviour
    {
        [SerializeField] private ParentConstraint _parentConstraint;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _parentConstraint.constraintActive = true;
            }
            else
            {
                _parentConstraint.constraintActive = false;
            }
        }
    }
}
