// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUIPanel : MonoBehaviour
    {
        public abstract string UIPanelName { get; }

        public abstract bool OverlayPreviousPanel { get; }
    }
}
