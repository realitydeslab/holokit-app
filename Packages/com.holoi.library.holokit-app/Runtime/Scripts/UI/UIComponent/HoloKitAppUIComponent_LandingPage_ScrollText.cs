// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_ScrollText : MonoBehaviour
    {
        [SerializeField] private RectTransform _text;

        private const float MovementSpeed = 120f;

        private void Update()
        {
            _text.anchoredPosition += new Vector2(MovementSpeed * Time.deltaTime, 0f);
        }
    }
}
