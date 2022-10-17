using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class LifeShieldVFXController : MonoBehaviour
    {
        [SerializeField] bool _isSingle = false;
        [SerializeField] GameObject _singleLifeShield;

        [SerializeField] LifeShield _lifeShield;

        [SerializeField] List<GameObject> _debrisExplosion; // 0 1 2 3 = top rigt left bot

        

        private void OnEnable()
        {
            if (!_isSingle)
            {
                LifeShield.OnTopDestroyed += OnShieldTopDestoryed;
                LifeShield.OnRightDestroyed += OnShieldRightDestoryed;
                LifeShield.OnLeftDestroyed += OnShieldLeftDestoryed;
                LifeShield.OnBotDestroyed += OnShieldBotDestoryed;
            }
            else
            {
                LifeShield.OnDead += OnSingleShieldDestoryed;
            }

        }

        private void OnDisable()
        {
            if (!_isSingle)
            {
                LifeShield.OnTopDestroyed -= OnShieldTopDestoryed;
                LifeShield.OnRightDestroyed -= OnShieldRightDestoryed;
                LifeShield.OnLeftDestroyed -= OnShieldLeftDestoryed;
                LifeShield.OnBotDestroyed -= OnShieldBotDestoryed;
            }
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

        void OnSingleShieldDestoryed(ulong ownerClientId)
        {
            if (ownerClientId == _lifeShield.OwnerClientId)
            {
                _singleLifeShield.SetActive(false);
                _debrisExplosion[0].SetActive(true);
                _debrisExplosion[1].SetActive(true);
                _debrisExplosion[2].SetActive(true);
                _debrisExplosion[3].SetActive(true);
            }
        }
    }
}
