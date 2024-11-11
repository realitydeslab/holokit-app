// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Runtime.InteropServices;
using UnityEngine.Localization.Settings;

namespace Holoi.Library.HoloKitAppLib.IOSNative
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

        public static void ShowUpdateAlert()
        {
            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "en":
                    HoloKitAppIOSNative_ShowFeedbackAlert("Update",
                        "There is a new version of HoloKit app on the app store.",
                        "Update Now",
                        "https://apps.apple.com/cn/app/holokit/id6444073276");
                    break;
                case "zh-Hans":
                    HoloKitAppIOSNative_ShowFeedbackAlert("更新",
                        "新版本已更新到App Store。",
                        "马上升级",
                        "https://apps.apple.com/cn/app/holokit/id6444073276");
                    break;
            }
        }
    }
}
