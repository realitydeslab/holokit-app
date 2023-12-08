// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUIPanel : MonoBehaviour
    {
        public abstract string UIPanelName { get; }

        public abstract bool OverlayPreviousPanel { get; }
    }
}
