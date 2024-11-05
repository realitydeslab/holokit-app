// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityDesignLab.Library.HoloKitApp.UI;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.TheGhost.UI
{
    public class MofaGhostUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MOFA.TheGhost";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] protected RectTransform TriggerButton;

        [Header("UI Panels")]
        [SerializeField] private HoloKitAppUIPanel _ghostUIPanel;

        private const float RotationSpeed = 20f;

        private void Start()
        {
            GhostSpawner.OnGhostSpawned += OnGhostSpawned;
        }

        private void OnDestroy()
        {
            GhostSpawner.OnGhostSpawned -= OnGhostSpawned;
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

        private void OnGhostSpawned()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel(_ghostUIPanel);
        }
    }
}
