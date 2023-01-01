using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.MOFATheHunting
{
    public class DragonEmberVFXController : MonoBehaviour
    {
        [SerializeField] GameObject Ember;
        [SerializeField] VisualEffect EmberVFX;

        int n = 0;

        void Start()
        {
            Ember.SetActive(false);
        }

        void Update()
        {
            n++;
            if (n == 1)
            {
                Ember.SetActive(true);
                //EmberVFX.Reinit();
            }
        }
    }
}
