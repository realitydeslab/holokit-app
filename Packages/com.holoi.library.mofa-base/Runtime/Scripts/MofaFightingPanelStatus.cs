using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelStatus : MonoBehaviour
    {
        private MofaBaseRealityManager _mofaRealityManager;

        [SerializeField] Animator _animator;
        [Header("UI Elements")]
        [SerializeField] VisualEffect _attackBar;
        [SerializeField] VisualEffect _ultimateBar;

        private void Awake()
        {
            //_mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

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
            // SIZHENGTODO:
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
            //_attackBar.SetFloat("Loading", _mofaRealityManager.LocalPlayerSpellManager.BasicSpellChargePercentage);
            //_ultimateBar.SetFloat("Loading", _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellChargePercentage);
            // SIZHENGTODO: 更新充能
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellUseCount 此为二技能已经使用过的次数
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpell.MaxUseCount 此为二技能的最大使用次数
        }
    }
}