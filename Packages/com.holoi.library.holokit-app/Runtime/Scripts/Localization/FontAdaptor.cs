// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Holoi.Library.HoloKitApp.Localization
{
    /// <summary>
    /// Attach this script to every TMPText component to enable font switch
    /// according to the current locale.
    /// </summary>
    public class FontAdaptor : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _englishFont;

        private const string EnglishCode = "en";

        private void Start()
        {
            var tmpText = GetComponent<TMP_Text>();

            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case EnglishCode:
                    tmpText.font = _englishFont;
                    break;
            }
        }
    }
}
