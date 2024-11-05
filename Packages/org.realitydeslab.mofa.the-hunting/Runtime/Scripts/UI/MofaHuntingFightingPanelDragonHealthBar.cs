// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheHunting.UI
{
    public class MofaHuntingFightingPanelDragonHealthBar : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            _material = GetComponent<Image>().material;
        }

        private void Update()
        {
            var mofaHuntingRealityManager = HoloKitApp.Instance.RealityManager as MofaHuntingRealityManager;
            var dragonController = mofaHuntingRealityManager.DragonController;
            if (dragonController != null)
            {
                _material.SetFloat("_Health", dragonController.CurrentHealthPercent);
            }
        }
    }
}
