// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_ExitButton : HoloKitAppUITemplate_StarAR_HorizontalScrollButton
    {
        public override bool SwipeRight => true;

        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        protected override void OnSelected()
        {
            base.OnSelected();
            _starARPanel.OnExitButtonPressed();
        }

        protected override void OnRecovered()
        {
            base.OnRecovered();
            _starARPanel.OnExitButtonReleased();
        }

        protected override void OnTriggerred()
        {
            base.OnTriggerred();
            HoloKitApp.Instance?.UIPanelManager.PopUIPanel();
            HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Mono;
        }
    }
}
