using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppWatchAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitAppWatch_InitializeWithRealityId(int realityId);
    }
}
