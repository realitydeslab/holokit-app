// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_WallCollisionController : MonoBehaviour, IDamageable
    {
        private void Start()
        {
            transform.position = HoloKitCameraManager.Instance.CenterEyePose.position + 8f * HoloKitCameraManager.Instance.CenterEyePose.forward;
            transform.LookAt(HoloKitCameraManager.Instance.CenterEyePose);

            Vector3 rotationEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y, 0f);
        }

        public void OnDamaged(ulong attackerClientId)
        {
            
        }
    }
}
