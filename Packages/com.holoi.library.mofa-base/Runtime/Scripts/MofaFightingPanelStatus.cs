using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanelStatus : MonoBehaviour
    {
        private MofaBaseRealityManager _mofaRealityManager;

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
                // 二技能开始充能
            }
            else
            {
                // 二技能停止充能
            }
        }

        private void Update()
        {
            // SIZHENGTODO: 更新充能
            // _mofaRealityManager.LocalPlayerSpellManager.BasicSpellChargePercentage 此为基础攻击的充能百分比
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellChargePercentage 此为二技能的充能百分比
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpellUseCount 此为二技能已经使用过的次数
            // _mofaRealityManager.LocalPlayerSpellManager.SecondarySpell.MaxUseCount 此为二技能的最大使用次数
        }
    }
}