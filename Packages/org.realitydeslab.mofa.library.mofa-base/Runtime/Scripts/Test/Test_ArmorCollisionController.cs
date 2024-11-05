// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace RealityDesignLab.MOFA.Library.MOFABase.Test
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
