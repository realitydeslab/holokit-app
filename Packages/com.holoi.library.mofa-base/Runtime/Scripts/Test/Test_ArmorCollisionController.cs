// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_ArmorCollisionController : MonoBehaviour, IDamageable
    {
        public void OnDamaged(ulong attackerClientId)
        {
            
        }

        private void LateUpdate()
        {
            transform.SetPositionAndRotation(HoloKitCameraManager.Instance.CenterEyePose.position, HoloKitCameraManager.Instance.CenterEyePose.rotation);
        }
    }
}
