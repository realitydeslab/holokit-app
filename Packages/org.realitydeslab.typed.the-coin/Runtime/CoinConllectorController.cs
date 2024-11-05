// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.Typed.TheCoin
{
    public class CoinConllectorController : MonoBehaviour
    {
        public Transform CoinCollector;

        public GameObject SucceeePrefab;


        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger enter");
            // kill the coin
            //Destroy(other.gameObject);
            other.transform.parent.gameObject.SetActive(false);
            AddScore();
        }

        void AddScore()
        {
           // FindObjectOfType<TypedTheCoinRealityManager>().Score += 1;
            var go = Instantiate(SucceeePrefab);
            go.transform.position = CoinCollector.position;
        }
    }
}
