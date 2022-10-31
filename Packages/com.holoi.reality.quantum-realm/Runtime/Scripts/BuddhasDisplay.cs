using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasDisplay : MonoBehaviour
    {
        private QuantumBuddhasSceneManager _manager;

        public List<BuddhasController> Buddhas = new List<BuddhasController>();

        private void Awake()
        {
            OnBuddhasDisplayBitrh();
            var manager = FindObjectOfType<QuantumBuddhasSceneManager>();
            manager.BuddhasDisPlay = this;
            manager.BuddhasTotalCount = Buddhas.Count;
        }

        void Start()
        {

        }

        void OnBuddhasDisplayBitrh()
        {
            var manager = FindObjectOfType<QuantumBuddhasSceneManager>();

            manager.vfxs.Clear();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var vfx = transform.GetChild(i).GetComponent<BuddhasController>().vfx;
                manager.vfxs.Add(vfx);
            }
        }

        public void DisableBuddha(int index)
        {
            Buddhas[index].gameObject.SetActive(false);
        }

        public void EnbaleBuddha(int index)
        {
            Buddhas[index].gameObject.SetActive(true);
        }
    }
}