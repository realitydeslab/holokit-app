// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
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
