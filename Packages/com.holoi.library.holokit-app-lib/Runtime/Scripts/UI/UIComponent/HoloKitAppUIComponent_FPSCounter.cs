// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fpsText;

        private const float UpdateInverval = 0.2f;

        private float _accumulatedTime = 0;

        private int _frameCount;

        private void OnEnable()
        {
            _fpsText.text = "60fps";
            _accumulatedTime = 0f;
            _frameCount = 0;
        }

        private void Update()
        {
            _accumulatedTime += Time.deltaTime;
            _frameCount++;
            if (_accumulatedTime > UpdateInverval)
            {
                int fps = Mathf.RoundToInt(_frameCount / _accumulatedTime);
                _fpsText.text = fps.ToString() + "fps";

                _accumulatedTime = 0f;
                _frameCount = 0;
            }
        }
    }
}
