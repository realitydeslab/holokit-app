// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System;

namespace RealityDesignLab.Library.HoloKitApp
{
    /// <summary>
    /// All Analytics events should be fired through this static class.
    /// </summary>
    public static class HoloKitAppAnalyticsEventManager
    {
        /// <summary>
        /// Sent when a player successfully signed in with their Apple ID for the first time.
        /// The first parameter is the user email and the second parameter is the user name.
        /// </summary>
        public static event Action<string, string> OnPlayerRegistered;

        /// <summary>
        /// Sent by the host player when a reality ends.
        /// </summary>
        public static event Action<RealitySessionData> OnDreamOver;

        /// <summary>
        /// Sent when a player's device gets overheated.
        /// </summary>
        public static event Action<HoloKitAppOverheatData> OnOverheated;

        public static void FireEvent_OnPlayerRegistered(string userEmail, string userName)
        {
            OnPlayerRegistered?.Invoke(userEmail, userName);
        }

        public static void FireEvent_OnDreamOver(RealitySessionData realitySessionData)
        {
            OnDreamOver?.Invoke(realitySessionData);
        }

        public static void FireEvent_OnOverheated(HoloKitAppOverheatData overheatData)
        {
            OnOverheated?.Invoke(overheatData);
        }
    }
}
