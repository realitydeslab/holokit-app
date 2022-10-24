using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelStatus : MonoBehaviour
    {
        [SerializeField] MofaInputManager _inputManager;

        [SerializeField] Animator _animator;

        [Header("UI Elements")]
        [SerializeField] VisualEffect _attackBar;

        [SerializeField] VisualEffect _ultimateBar;

        private void OnEnable()
        {
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
        }

        private void OnDisable()
        {
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            if (watchState == MofaWatchState.Ground)
            {
                _animator.SetTrigger("To Function");
            }
            else
            {
                _animator.SetTrigger("To Attack");
            }
        }

        private void Update()
        {
            if (_inputManager.Active)
            {
                //_attackBar.SetFloat("Loading", _inputManager.BasicSpellChargePercentage);
                //_ultimateBar.SetFloat("Loading", _inputManager.SecondarySpellChargePercentage);
            }
        }
    }
}