// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.MOFA.TheHunting
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
