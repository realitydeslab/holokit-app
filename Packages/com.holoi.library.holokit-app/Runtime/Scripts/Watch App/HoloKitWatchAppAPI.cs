using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp.Watch
{
    public static class HoloKitWatchAppAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitWatchApp_InitializeWithRealityId(int realityId);

        public static void InitializeWithRealityId(int realityId)
        {
            HoloKitWatchApp_InitializeWithRealityId(realityId);
        }
    }
}
