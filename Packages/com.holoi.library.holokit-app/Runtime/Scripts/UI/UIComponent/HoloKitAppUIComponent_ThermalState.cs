// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_ThermalState : MonoBehaviour
    {
        [SerializeField] private Image _thermalStateImage;

        private readonly Color32 _normalColor = Color.blue;

        private readonly Color32 _fairColor = Color.green;

        private readonly Color32 _seriousColor = Color.yellow;

        private readonly Color32 _criticalColor = Color.red;

        private void OnEnable()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitARSessionControllerAPI.OnThermalStateChanged += OnThermalStateChanged;
                OnThermalStateChanged(HoloKitARSessionControllerAPI.GetThermalState());
            }
        }

        private void OnDisable()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitARSessionControllerAPI.OnThermalStateChanged -= OnThermalStateChanged;
            }
        }

        private void OnThermalStateChanged(ThermalState thermalState)
        {
            switch (thermalState)
            {
                case ThermalState.ThermalStateNominal:
                    _thermalStateImage.color = _normalColor;
                    break;
                case ThermalState.ThermalStateFair:
                    _thermalStateImage.color = _fairColor;
                    break;
                case ThermalState.ThermalStateSerious:
                    _thermalStateImage.color = _seriousColor;
                    break;
                case ThermalState.ThermalStateCritical:
                    _thermalStateImage.color = _criticalColor;
                    break;
            }
        }
    }
}
