// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typed.TheMimic
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] Transform _joint;
        public bool _force;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (_force)
            {
                _force = false;
                OnAppleForce();
            }
        }

        void OnAppleForce()
        {
            _joint.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
        }
    }
}
