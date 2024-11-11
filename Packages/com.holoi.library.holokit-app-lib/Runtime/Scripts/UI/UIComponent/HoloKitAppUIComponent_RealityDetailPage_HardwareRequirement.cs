// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
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
