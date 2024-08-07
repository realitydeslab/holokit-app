// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.HoloKitApp.UI
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
