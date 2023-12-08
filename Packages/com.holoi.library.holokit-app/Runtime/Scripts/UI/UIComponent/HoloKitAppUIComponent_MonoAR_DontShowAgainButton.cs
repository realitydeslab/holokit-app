// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;

namespace Holoi.Library.HoloKitApp.UI
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
