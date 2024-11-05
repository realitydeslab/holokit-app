// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;

namespace RealityDesignLab.Library.HoloKitApp.UI
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
