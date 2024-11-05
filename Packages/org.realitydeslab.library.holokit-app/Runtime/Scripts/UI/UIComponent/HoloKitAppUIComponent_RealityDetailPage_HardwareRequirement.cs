// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_HardwareRequirement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _content;

        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (currentReality.HardwareRequirement.Equals(""))
            {
                Destroy(gameObject);
                return;
            }

            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "en":
                    _content.text = currentReality.HardwareRequirement;
                    break;
                case "zh-Hans":
                    _content.text = currentReality.HardwareRequirement_Chinese;
                    break;
            }
        }
    }
}
