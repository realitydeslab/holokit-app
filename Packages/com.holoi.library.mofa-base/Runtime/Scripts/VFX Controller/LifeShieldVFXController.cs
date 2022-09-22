using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Mofa.Base
{
    public class LifeShieldVFXController : MonoBehaviour
    {
        //[SerializeField] LifeShield _lifeShieldController;
        [SerializeField] List<GameObject> _debrisExplosion; // 0 1 2 3 = top rigt left bot

        private void OnEnable()
        {
            //yc todo:
            LifeShield.OnTopDestroyed += OnShieldTopDestoryed;
            LifeShield.OnRightDestroyed += OnShieldTopDestoryed;
            LifeShield.OnLeftDestroyed += OnShieldTopDestoryed;
            LifeShield.OnBotDestroyed += OnShieldTopDestoryed;
        }

        private void OnDisable()
        {
            //yc todo:
            LifeShield.OnTopDestroyed -= OnShieldTopDestoryed;
            LifeShield.OnRightDestroyed -= OnShieldTopDestoryed;
            LifeShield.OnLeftDestroyed -= OnShieldTopDestoryed;
            LifeShield.OnBotDestroyed -= OnShieldTopDestoryed;
        }

        void OnShieldTopDestoryed(ulong id)
        {
            _debrisExplosion[0].SetActive(true);
        }
        void OnShieldRightDestoryed(ulong id)
        {
            _debrisExplosion[1].SetActive(true);
        }
        void OnShieldLeftDestoryed(ulong id)
        {
            _debrisExplosion[2].SetActive(true);
        }
        void OnShieldBotDestoryed(ulong id)
        {
            _debrisExplosion[3].SetActive(true);
        }
    }
}
