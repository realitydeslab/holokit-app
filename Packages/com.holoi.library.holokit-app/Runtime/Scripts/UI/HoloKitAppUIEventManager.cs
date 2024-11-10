// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;

namespace Holoi.Library.HoloKitApp.UI
{
    public static class HoloKitAppUIEventManager
    {
        #region Reality Settings Panel
        public static Action<bool> OnHumanOcclusionToggled;
        #endregion

        #region Star AR Panel
        public static Action OnStarUITriggered;

        public static Action OnStarUIBoosted;
        #endregion
    }
}