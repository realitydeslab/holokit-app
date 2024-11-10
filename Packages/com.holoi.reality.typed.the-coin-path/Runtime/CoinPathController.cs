// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typed.TheCoinPath
{
    public class CoinPathController : MonoBehaviour
    {
        [SerializeField] List<GameObject> CoinsInPath = new();

        void Start()
        {
        
        }

        void Update()
        {

        }

        public void Reset()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
