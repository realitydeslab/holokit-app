// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine.Localization.Settings;

namespace Holoi.Library.HoloKitApp
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
