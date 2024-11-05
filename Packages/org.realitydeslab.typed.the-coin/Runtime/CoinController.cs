// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityDesignLab.Library.ARUX;

namespace RealityDesignLab.Typed.TheCoin
{
    public class CoinController : MonoBehaviour
    {
        public float Speed = 1;
        public RotateOverTime ROT;
        // Start is called before the first frame update
        void Start()
        {
            //ROT.RotateVector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
            GetComponent<Rigidbody>().velocity = transform.forward * Speed;
        }

        // Update is called once per frame
        void Update()
        {
            //transform.position += transform.forward * Speed * Time.deltaTime;
        }
    }
}
