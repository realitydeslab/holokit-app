// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public abstract class HoloKitAppUIPanel : MonoBehaviour
    {
        public abstract string UIPanelName { get; }

        public abstract bool OverlayPreviousPanel { get; }
    }
}
