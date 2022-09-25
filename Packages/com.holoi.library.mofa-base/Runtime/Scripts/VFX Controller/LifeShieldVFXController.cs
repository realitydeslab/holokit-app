using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class LifeShieldVFXController : MonoBehaviour
    {
        [SerializeField] LifeShield _lifeShield;

        [SerializeField] List<GameObject> _debrisExplosion; // 0 1 2 3 = top rigt left bot

        private void OnEnable()
        {
            LifeShield.OnTopDestroyed += OnShieldTopDestoryed;
            LifeShield.OnRightDestroyed += OnShieldRightDestoryed;
            LifeShield.OnLeftDestroyed += OnShieldLeftDestoryed;
            LifeShield.OnBotDestroyed += OnShieldBotDestoryed;
        }

        private void OnDisable()
        {
            LifeShield.OnTopDestroyed -= OnShieldTopDestoryed;
            LifeShield.OnRightDestroyed -= OnShieldRightDestoryed;
            LifeShield.OnLeftDestroyed -= OnShieldLeftDestoryed;
            LifeShield.OnBotDestroyed -= OnShieldBotDestoryed;
        }

        void OnShieldTopDestoryed(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.OwnerClientId)
            {
                _debrisExplosion[0].SetActive(true);
            }
        }
        void OnShieldRightDestoryed(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.OwnerClientId)
            {
                _debrisExplosion[1].SetActive(true);
            }
        }
        void OnShieldLeftDestoryed(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.OwnerClientId)
            {
                _debrisExplosion[2].SetActive(true);
            }
        }
        void OnShieldBotDestoryed(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.OwnerClientId)
            {
                _debrisExplosion[3].SetActive(true);
            }
        }
    }
}
