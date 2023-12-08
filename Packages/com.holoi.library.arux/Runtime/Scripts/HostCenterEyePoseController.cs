// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Animations;

namespace Holoi.Library.ARUX
{
    public class HostCenterEyePoseController : MonoBehaviour
    {
        [SerializeField] private ParentConstraint _parentConstraint;

        private void Start()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsHost)
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
