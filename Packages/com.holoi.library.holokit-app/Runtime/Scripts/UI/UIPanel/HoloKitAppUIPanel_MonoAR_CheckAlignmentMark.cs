// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_CheckAlignmentMark : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_CheckAlignmentMark";

        public override bool OverlayPreviousPanel => true;

        public void OnCheckedButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();

            HoloKitApp.Instance.MultiplayerManager.OnCheckAlignmentMarker();
        }

        public void OnRescanButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");

            HoloKitApp.Instance.MultiplayerManager.OnRescanQRCode();
        }
    }
}
