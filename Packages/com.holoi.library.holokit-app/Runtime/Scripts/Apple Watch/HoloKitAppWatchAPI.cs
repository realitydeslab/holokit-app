using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp.Watch
{
    public static class HoloKitAppWatchAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitAppWatch_InitializeWithRealityId(int realityId);

        public static void InitializeWithRealityId(int realityId)
        {
            HoloKitAppWatch_InitializeWithRealityId(realityId);
        }
    }
}
