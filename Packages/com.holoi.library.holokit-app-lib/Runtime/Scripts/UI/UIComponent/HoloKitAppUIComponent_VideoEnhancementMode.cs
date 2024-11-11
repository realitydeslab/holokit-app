// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_VideoEnhancementMode : MonoBehaviour
    {
        [SerializeField] private TMP_Text _videoEnhancementModeText;

        private void OnEnable()
        {
            if (HoloKitCameraManager.Instance.VideoEnhancementMode == VideoEnhancementMode.HighResWithHDR)
            {
                _videoEnhancementModeText.text = "4KHDR";
            }
            else if (HoloKitCameraManager.Instance.VideoEnhancementMode == VideoEnhancementMode.HighRes)
            {
                _videoEnhancementModeText.text = "4K";
            }
            else
            {
                _videoEnhancementModeText.text = "2K";
            }
        }
    }
}
