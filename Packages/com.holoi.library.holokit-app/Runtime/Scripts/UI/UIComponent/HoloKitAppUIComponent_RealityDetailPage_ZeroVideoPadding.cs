// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
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
