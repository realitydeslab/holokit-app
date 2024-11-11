// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_MonoAR_DontShowAgainButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private HoloKitAppUIPanel_MonoAR_GettingStarted _gettingStartedPanel;

        public void OnPointerDown(PointerEventData eventData)
        {
            _gettingStartedPanel.OnToggleChecked();
        }
    }
}
