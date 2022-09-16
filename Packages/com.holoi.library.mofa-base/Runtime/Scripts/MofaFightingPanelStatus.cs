using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using UnityEngine.VFX;

namespace Holoi.Mofa.Base
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
            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        private void OnEnable()
        {
            LocalPlayerSpellManager.OnControllerStateChanged += OnControllerStateChanged;
        }

        private void OnDisable()
        {
            LocalPlayerSpellManager.OnControllerStateChanged -= OnControllerStateChanged;
        }

        private void OnControllerStateChanged(ControllerState controllerState)
        {
            // SIZHENGTODO:
            if (controllerState == ControllerState.Ground)
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
            _attackBar.SetFloat("Loading", _mofaRealityManager.LocalPlayerSpellManager.BasicSpellChargePercentage);
            _ultimateBar.SetFloat("Loading", _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellChargePercentage);
            // SIZHENGTODO: 更新充能
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellUseCount 此为二技能已经使用过的次数
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpell.MaxUseCount 此为二技能的最大使用次数
        }
    }
}