// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public static class MofaUtils
    {
        public const float MeterToFoot = 3.2808f;

        // TODO: Find a professional value
        public const float MeterToKcal = 0.1f;

        public static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }

        public static Vector3 GetHorizontalRight(Transform transform)
        {
            return new Vector3(transform.right.x, 0f, transform.right.z).normalized;
        }

        public static Quaternion GetHorizontalRotation(Quaternion rotation)
        {
            Vector3 cameraEuler = rotation.eulerAngles;
            return Quaternion.Euler(new Vector3(0f, cameraEuler.y, 0f));
        }

        public static Quaternion GetHorizontalLookRotation(Vector3 forward)
        {
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            return GetHorizontalRotation(lookRotation);
        }
    }
}