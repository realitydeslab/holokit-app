// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFA.TheGhost.UI
{
    public class GhostPlayerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MOFA.TheGhost_Ghost";

        public override bool OverlayPreviousPanel => false;

        public static event Action OnTriggered;

        private void Start()
        {
            transform.SetAsFirstSibling();
        }

        public void OnTriggeredFunc()
        {
            OnTriggered?.Invoke();
        }
    }
}
