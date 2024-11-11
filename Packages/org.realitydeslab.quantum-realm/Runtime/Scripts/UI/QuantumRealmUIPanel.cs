// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Holoi.Library.HoloKitAppLib.UI;

namespace RealityDesignLab.QuantumRealm.UI
{
    public class QuantumRealmUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "QuantumRealm";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private RectTransform _switchButton;

        private const float RotationSpeed = 20f;

        public static event Action OnSwitchButtonPressed;

        private void Update()
        {
            _switchButton.Rotate(new Vector3(0f, 0f, 1f), -RotationSpeed * Time.deltaTime);
        }

        public void OnSwitchButtonPressedFunc()
        {
            OnSwitchButtonPressed?.Invoke();
        }
    }
}
