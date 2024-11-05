// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_PermissionsPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "PermissionsPage";

        public override bool OverlayPreviousPanel => true;

        private void Start()
        {
            if (HoloKitAppPermissionsManager.MandatoryPermissionsGranted())
            {
                HoloKitApp.Instance.UIPanelManager.PopUIPanel();
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("SignInPage");
            }
        }
    }
}
