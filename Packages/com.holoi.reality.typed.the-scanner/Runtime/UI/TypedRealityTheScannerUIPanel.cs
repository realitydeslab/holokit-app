// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using TMPro;

namespace Holoi.Reality.Typography
{
    public class TypedRealityTheScannerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TypedRealityTheScanner";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private GameObject _text;

        [SerializeField] private Color _textColor;

        private void Start()
        {
            Color newColor = new(_textColor.r, _textColor.g, _textColor.b, 0f);
            LeanTween.value(_text, (Color color) => { _text.GetComponent<TMP_Text>().color = color; }, _textColor, newColor, 2f)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(4f)
                .setOnComplete(() =>
                {
                    _text.SetActive(false);
                });
        }
    }
}
