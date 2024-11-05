// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_ZeroVideoPadding : MonoBehaviour
    {
        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (currentReality.PreviewVideos.Count != 0 || currentReality.TutorialVideos.Count != 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
