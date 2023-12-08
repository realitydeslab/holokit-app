// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase.UI
{
    public class MofaUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "Mofa";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] protected RectTransform TriggerButton;

        private const float RotationSpeed = 20f;

        protected virtual void Start()
        {
            // Disable the trigger button on spectators
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
                TriggerButton.gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (TriggerButton.gameObject.activeSelf)
                TriggerButton.Rotate(new Vector3(0f, 0f, 1f), -RotationSpeed * Time.deltaTime);
        }

        public void OnTriggerButtonPressed()
        {
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
        }
    }
}
