// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.MOFABase
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
