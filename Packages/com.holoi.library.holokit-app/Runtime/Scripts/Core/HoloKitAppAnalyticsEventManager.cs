using System;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// All Analytics events should be fired through this static class.
    /// </summary>
    public static class HoloKitAppAnalyticsEventManager
    {
        /// <summary>
        /// Sent by the host player when a reality ends.
        /// </summary>
        public static event Action<RealitySessionData> OnDreamOver;

        /// <summary>
        /// Sent when a player's device gets overheated.
        /// </summary>
        public static event Action<HoloKitAppOverheatData> OnOverheated;

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
