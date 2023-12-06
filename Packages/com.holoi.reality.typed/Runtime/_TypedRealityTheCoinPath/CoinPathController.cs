using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
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
