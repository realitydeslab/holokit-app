// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine.Localization.Settings;

namespace RealityDesignLab.Library.HoloKitApp
{
    public enum SupportedLanguage
    {
        English = 0,
        Chinese = 1
    }

    public static class HoloKitAppLocalizationManager
    {
        public static SupportedLanguage GetCurrentLanguage()
        {
            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "en":
                    return SupportedLanguage.English;
                case "zh-Hans":
                    return SupportedLanguage.Chinese;
                default:
                    return SupportedLanguage.English;
            }
        }
    }
}
