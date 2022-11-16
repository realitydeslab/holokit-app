using System.Collections;
using UnityEngine;
using MalbersAnimations;

namespace Holoi.Reality.MOFATheHunting
{
    public class PolyDragonController : MonoBehaviour
    {
        [SerializeField] private FireBreath _fireBreath;

        private void Start()
        {
            StartCoroutine(PeriodicFireBreath());
        }

        private IEnumerator PeriodicFireBreath()
        {
            while(true)
            {
                yield return new WaitForSeconds(2f);
                _fireBreath.Activate(true);
                yield return new WaitForSeconds(2f);
                _fireBreath.Activate(false);
            }
        }

        public void PlaySound()
        {

        }
    }
}
