// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityDesignLab.Library.HoloKitApp.UI
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
