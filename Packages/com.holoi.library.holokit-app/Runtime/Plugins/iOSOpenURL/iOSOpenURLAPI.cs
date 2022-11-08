using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp.iOSOpenURL
{
    public static class iOSOpenURLAPI
    {
        [DllImport("__Internal")]
        private static extern void iOSOpenURL_OpenURL(string url);

        public static void OpenURL(string url)
        {
            iOSOpenURL_OpenURL(url);
        }
    }
}
