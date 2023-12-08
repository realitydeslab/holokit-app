// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_EnterRealityButton : MonoBehaviour
    {
        [SerializeField] private Image _ringTextImage;

        private const float RotationSpeed = 30f;

        private void Update()
        {
            _ringTextImage.transform.Rotate(new Vector3(0f, 0f, 1f), -RotationSpeed * Time.deltaTime);
        }

        public void OnEnterRealityButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityPreferencesPage");
        }
    }
}
