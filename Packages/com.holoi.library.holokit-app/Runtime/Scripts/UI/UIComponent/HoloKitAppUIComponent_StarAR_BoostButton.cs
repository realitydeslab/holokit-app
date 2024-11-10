// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_BoostButton : HoloKitAppUITemplate_StarAR_HorizontalScrollButton
    {
        public override bool SwipeRight => true;

        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        protected override void OnSelected()
        {
            base.OnSelected();

            _starARPanel.OnBoostButtonPressed();
        }

        protected override void OnRecovered()
        {
            base.OnRecovered();

            _starARPanel.OnBoostButtonReleased();
        }

        protected override void OnTriggerred()
        {
            base.OnTriggerred();

            HoloKitAppUIEventManager.OnStarUIBoosted?.Invoke();
        }
    }
}
