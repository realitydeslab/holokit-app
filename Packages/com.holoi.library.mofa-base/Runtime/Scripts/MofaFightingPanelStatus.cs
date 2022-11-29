using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelStatus : MonoBehaviour
    {
        [SerializeField] private MofaInputManager _inputManager;

        [SerializeField] private Animator _animator;

        [Header("UI Elements")]
        [SerializeField] private VisualEffect _lifeCircles;

        [SerializeField] private VisualEffect _attackBar;

        [SerializeField] private VisualEffect _ultimateBar;

        // The life shield of the local player.
        private LifeShield _lifeShield;

        private void Start()
        {
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
            LifeShield.OnRenovated += OnLifeShieldRenovated;
        }

        private void OnDestroy()
        {
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
            LifeShield.OnRenovated -= OnLifeShieldRenovated;
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            if (!gameObject.activeSelf) return;

            if (watchState == MofaWatchState.Ground)
            {
                _animator.SetBool("Attack", false);
            }
            else
            {
                _animator.SetBool("Attack", true);
            }
        }

        private void Update()
        {
            if (_inputManager.IsActive)
            {
                _attackBar.SetFloat("Loading Process", _inputManager.BasicSpellChargePercentage);
                _ultimateBar.SetFloat("Loading Process", _inputManager.SecondarySpellChargePercentage);
            }
        }

        public void SetLifeShield(LifeShield lifeShield)
        {
            _lifeShield = lifeShield;
            _lifeShield.OnCenterDestroyed += OnLifeShieldCenterDestroyed;
            _lifeShield.OnTopDestroyed += OnLifeShieldTopDestroyed;
            _lifeShield.OnLeftDestroyed += OnLifeShieldLeftDestroyed;
            _lifeShield.OnRightDestroyed += OnLifeShieldRightDestroyed;
        }

        private void OnLifeShieldRenovated(ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                _lifeCircles.SetBool("Center", true);
                _lifeCircles.SetBool("Up", true);
                _lifeCircles.SetBool("Left", true);
                _lifeCircles.SetBool("Right", true);
            }
        }

        private void OnLifeShieldCenterDestroyed()
        {
            _lifeCircles.SetBool("Center", false);
        }

        private void OnLifeShieldTopDestroyed()
        {
            _lifeCircles.SetBool("Up", false);
        }

        private void OnLifeShieldLeftDestroyed()
        {
            _lifeCircles.SetBool("Left", false);
        }

        private void OnLifeShieldRightDestroyed()
        {
            _lifeCircles.SetBool("Right", false);
        }
    }
}