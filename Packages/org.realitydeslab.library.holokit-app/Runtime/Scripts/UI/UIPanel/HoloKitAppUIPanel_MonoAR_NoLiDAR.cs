// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_NoLiDAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_NoLiDAR";

        public override bool OverlayPreviousPanel => true;

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.ExitNoLiDARScene();
        }
    }
}
