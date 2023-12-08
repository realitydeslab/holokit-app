// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasHandController : MonoBehaviour
    {
        public GameObject HandVisual;
        public GameObject ExplodeVFX;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void OnExplode()
        {
            HandVisual.SetActive(false);
            ExplodeVFX.SetActive(true);

            StartCoroutine(WaitAndEnableGO(HandVisual));
            StartCoroutine(WaitAndDisableGO(ExplodeVFX));
        }

        IEnumerator WaitAndEnableGO(GameObject GO)
        {
            yield return new WaitForSeconds(3f);
            GO.SetActive(true);
        }

        IEnumerator WaitAndDisableGO(GameObject GO)
        {
            yield return new WaitForSeconds(3f);
            GO.SetActive(false);
        }
    }
}
