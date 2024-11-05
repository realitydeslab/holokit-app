// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class DragonEmberVFXController : MonoBehaviour
    {
        [SerializeField] GameObject Ember;
        [SerializeField] VisualEffect EmberVFX;

        int n = 0;

        void Start()
        {
            Ember.SetActive(false);
        }

        void Update()
        {
            n++;
            if (n == 1)
            {
                Ember.SetActive(true);
                //EmberVFX.Reinit();
            }
        }
    }
}
