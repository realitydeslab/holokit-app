using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Reality.Typography.UI
{
    public class TheFingerRibbonUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TheFingerRibbonUIPanel";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private RectTransform _text;

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
                LeanTween.alphaText(_text, 0f, FadeDuration);
            } 
        }
    }
}
