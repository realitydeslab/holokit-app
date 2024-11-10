// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;
using TMPro;

namespace Holoi.Reality.Typed.TheFingerRibbon
{
    public class TheFingerRibbonUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TheFingerRibbonUIPanel";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private TMP_Text _text;

        private bool _isFaded = false;

        private const float FadeDuration = 1f;

        private void Start()
        {
            HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
        }

        private void OnDestroy()
        {
            HoloKitHandTracker.OnHandValidityChanged -= OnHandValidityChanged;
        }

        private void OnHandValidityChanged(bool isValid)
        {
            if (isValid && !_isFaded)
            {
                _isFaded = true;
                Color originalColor = _text.color;
                LeanTween.value(1f, 0f, FadeDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnUpdate((float alpha) =>
                    {
                        _text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    })
                    .setOnComplete(() =>
                    {
                        _text.gameObject.SetActive(false);
                    });
            }
        }
    }
}
