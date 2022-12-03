using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
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
            Destroy(other.gameObject);
            AddScore();
        }

        void AddScore()
        {
            var go = Instantiate(SucceeePrefab);
            go.transform.position = CoinCollector.position;
        }
    }
}
