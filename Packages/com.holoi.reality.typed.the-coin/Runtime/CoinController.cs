// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typed.TheCoin
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
