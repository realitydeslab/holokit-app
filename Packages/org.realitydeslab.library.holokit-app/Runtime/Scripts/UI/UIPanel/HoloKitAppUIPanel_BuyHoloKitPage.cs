// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_BuyHoloKitPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "BuyHoloKitPage";

        public override bool OverlayPreviousPanel => true;

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnOrderHoloKitButtonPressed()
        {
            // TODO: Jump to Safari
        }
    }
}
