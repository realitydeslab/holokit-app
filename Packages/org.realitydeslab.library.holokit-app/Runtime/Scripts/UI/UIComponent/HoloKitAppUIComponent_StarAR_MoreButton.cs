// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_MoreButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        public void OnPointerDown(PointerEventData eventData)
        {
            _starARPanel.OnMoreButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _starARPanel.OnMoreButtonReleased();
        }
    }
}
