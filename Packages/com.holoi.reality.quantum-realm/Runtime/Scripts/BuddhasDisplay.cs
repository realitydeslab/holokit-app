using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasDisplay : MonoBehaviour
    {
        private void Awake()
        {
            OnBuddhasDisplayBitrh();
        }

        void Start()
        {
        
        }

        public void OnBuddhasDisplayBitrh()
        {
            var manager = FindObjectOfType<QuantumBuddhasSceneManager>();

            manager.vfxs.Clear();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var vfx = transform.GetChild(i).GetComponent<BuddhasController>().vfx;
                manager.vfxs.Add(vfx);
            }
        }
    }
}
