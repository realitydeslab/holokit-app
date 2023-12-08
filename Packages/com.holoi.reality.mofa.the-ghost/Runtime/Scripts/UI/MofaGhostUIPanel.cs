// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFA.TheGhost.UI
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
