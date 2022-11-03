using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class LifeShieldVFXController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _debrisExplosion; // 0 1 2 3 = center top left right

        private LifeShield _lifeShield;

        private void Start()
        {
            _lifeShield = GetComponentInParent<LifeShield>();
            _lifeShield.OnCenterDestroyed += OnShieldCenterDestroyed;
            _lifeShield.OnTopDestroyed += OnShieldTopDestroyed;
            _lifeShield.OnLeftDestroyed += OnShieldLeftDestroyed;
            _lifeShield.OnRightDestroyed += OnShieldRightDestroyed;
        }

        private void OnDestroy()
        {
            _lifeShield.OnCenterDestroyed -= OnShieldCenterDestroyed;
            _lifeShield.OnTopDestroyed -= OnShieldTopDestroyed;
            _lifeShield.OnLeftDestroyed -= OnShieldLeftDestroyed;
            _lifeShield.OnRightDestroyed -= OnShieldRightDestroyed;
        }

        private void OnShieldCenterDestroyed()
        {
            _debrisExplosion[0].SetActive(true);
        }

        private void OnShieldTopDestroyed()
        {
            _debrisExplosion[1].SetActive(true);
        }

        private void OnShieldLeftDestroyed()
        {
            _debrisExplosion[2].SetActive(true);
        }

        private void OnShieldRightDestroyed()
        {
            _debrisExplosion[3].SetActive(true);
        }
    }
}
