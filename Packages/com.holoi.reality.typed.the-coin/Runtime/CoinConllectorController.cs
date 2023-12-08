// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typed.TheCoin
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
