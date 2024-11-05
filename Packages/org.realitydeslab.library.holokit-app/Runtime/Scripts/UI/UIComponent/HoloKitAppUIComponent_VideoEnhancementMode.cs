// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;
using HoloKit;

namespace RealityDesignLab.Library.HoloKitApp.UI
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
