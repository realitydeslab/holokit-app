// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_GettingStartedPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "GettingStartedPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnVerifyHoloKitButtonPressed()
        {
            // TODO:
        }

        public void OnOrderHoloKitButtonPressed()
        {
            // TODO:
        }
    }
}
