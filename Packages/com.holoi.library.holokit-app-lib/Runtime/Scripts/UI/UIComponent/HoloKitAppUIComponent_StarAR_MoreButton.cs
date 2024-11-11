// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitAppLib.UI
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
