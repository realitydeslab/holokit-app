// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.HoloKitAppLib.UI
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
