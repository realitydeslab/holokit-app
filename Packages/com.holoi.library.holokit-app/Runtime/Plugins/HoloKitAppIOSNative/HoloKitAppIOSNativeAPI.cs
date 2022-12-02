using System.Runtime.InteropServices;

namespace Holoi.Library.HoloKitApp.IOSNative
{
    public static class HoloKitAppIOSNativeAPI
    {
        [DllImport("__Internal")]
        private static extern void HoloKitAppIOSNative_OpenURL(string url);

        [DllImport("__Internal")]
        private static extern void HoloKitAppIOSNative_ShowFeedbackAlert(string title, string message, string actionTile, string url);

        public static void OpenURL(string url)
        {
            HoloKitAppIOSNative_OpenURL(url);
        }

        public static void ShowFeedbackAlert()
        {
            HoloKitAppIOSNative_ShowFeedbackAlert("Feedback",
                "Please go to our discord to give your feedback.",
                "Go to Discord",
                "https://discord.gg/nsPPBfAJ2f");
        }
    }
}
