// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typed.TheCoin
{
    public class CoinMachineController : MonoBehaviour
    {
        public List<Transform> CoinSpawnerGroup = new();
        public GameObject CoinPrefab;
        public Transform CoinInstanceContainer;
        public float SpawnPossibility = 0.5f; 

        GameObject _coinInstance;

        void Start()
        {
            //StartCoroutine(WaitAndSpawnCoins());

            foreach (var coinSpawner in CoinSpawnerGroup)
            {
                StartCoroutine(WaitAndStart(coinSpawner, Random.Range(0f, 1f)));
            }
        }

        void Update()
        {

        }

        IEnumerator WaitAndStart(Transform coinSpawner, float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(WaitAndSpawnCoin(coinSpawner));
        }

        IEnumerator WaitAndSpawnCoins()
        {
            yield return new WaitForSeconds(1f);

            foreach (var coinSpawner in CoinSpawnerGroup)
            {
                if (Random.Range(0f, 1f) > (1 - SpawnPossibility))
                {
                    _coinInstance = Instantiate(CoinPrefab, CoinInstanceContainer);
                    _coinInstance.transform.position = coinSpawner.position;
                    var lookatPos = _coinInstance.transform.position + coinSpawner.forward;
                    _coinInstance.transform.LookAt(lookatPos);
                }
            }

            StartCoroutine(WaitAndSpawnCoins());
        }

        IEnumerator WaitAndSpawnCoin(Transform spawner)
        {
            yield return new WaitForSeconds(1f);

            if (Random.Range(0f, 1f) > (1 - SpawnPossibility))
            {
                _coinInstance = Instantiate(CoinPrefab, CoinInstanceContainer);
                _coinInstance.transform.position = spawner.position;
                var lookatPos = _coinInstance.transform.position + spawner.forward;
                _coinInstance.transform.LookAt(lookatPos);
            }

            StartCoroutine(WaitAndSpawnCoin(spawner));
        }
    }
}
