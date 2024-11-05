// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;
using HoloKit;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private void OnEnable()
        {
            UpdateTimer();
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            float sessionDuration = Time.time - HoloKitCameraManager.Instance.ARSessionStartTime;
            _timerText.text = HoloKitAppUtils.SecondToMMSS(sessionDuration);
        }
    }
}
