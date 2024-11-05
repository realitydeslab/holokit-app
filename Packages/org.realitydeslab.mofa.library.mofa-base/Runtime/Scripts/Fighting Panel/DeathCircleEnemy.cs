// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public class DeathCircleEnemy : MonoBehaviour
    {
        private float _accumulatedTime;

        private const float Lifetime = 3f;

        private void Update()
        {
            _accumulatedTime += Time.deltaTime;

            if (_accumulatedTime > Lifetime)
                Destroy(gameObject);
        }
    }
}
