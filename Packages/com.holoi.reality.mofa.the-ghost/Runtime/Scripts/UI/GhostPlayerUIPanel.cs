// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
