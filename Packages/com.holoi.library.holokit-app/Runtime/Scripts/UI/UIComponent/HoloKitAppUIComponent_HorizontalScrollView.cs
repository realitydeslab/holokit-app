// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Events;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_HorizontalScrollView : MonoBehaviour
    {
        [SerializeField] private RectTransform _touchArea;

        [SerializeField] private RectTransform _scrollRoot;

        private float _touchBeganTime;

        private Vector2 _touchBeganPosition;

        private int _currentStep;

        private const float SwiftSwipeThreshold = 1200f;

        private const float CompleteLeanTweenDuration = 0.5f;

        public UnityEvent<int> OnStepChanged;

        private void OnEnable()
        {
            _currentStep = 0;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (!IsTouchValid(touch.position))
                    {
                        return;
                    }
                    _touchBeganTime = Time.time;
                    _touchBeganPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    LeanTween.cancel(_scrollRoot);
                    float elementWidth = _scrollRoot.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

                    // If this is a swift swipe
                    float swipeX = touch.position.x - _touchBeganPosition.x;
                    if (Mathf.Abs(swipeX) / (Time.time - _touchBeganTime) > SwiftSwipeThreshold)
                    {
                        _currentStep = swipeX > 0 ? _currentStep - 1 : _currentStep + 1;
                        if (_currentStep < 0)
                        {
                            _currentStep = 0;
                        }
                        else if (_currentStep > _scrollRoot.childCount - 1)
                        {
                            _currentStep = _scrollRoot.childCount - 1;
                        }
                    }
                    else
                    {
                        _currentStep = Mathf.RoundToInt(-_scrollRoot.anchoredPosition.x / elementWidth);
                    }
                    float diff = Mathf.Abs(_scrollRoot.anchoredPosition.x + _currentStep * elementWidth);
                    float leanTweenDuration = diff / elementWidth * CompleteLeanTweenDuration;
                    LeanTween.moveX(_scrollRoot, -_currentStep * elementWidth, leanTweenDuration)
                        .setEase(LeanTweenType.easeInQuad)
                        .setOnComplete(() =>
                        {
                            OnStepChanged?.Invoke(_currentStep);
                        });
                }
            }
        }

        /// <summary>
        /// A touch is valid only if it is inside the predefined touch area.
        /// </summary>
        /// <param name="position">The 2D position of the touch</param>
        /// <returns>True for valid touches and false for invalid</returns>
        private bool IsTouchValid(Vector2 position)
        {
            if (position.x > (_touchArea.position.x - _touchArea.sizeDelta.x / 2f)
                && position.x < (_touchArea.position.x + _touchArea.sizeDelta.x / 2f)
                &&
                position.y > (_touchArea.position.y - _touchArea.sizeDelta.y / 2f)
                && position.y < (_touchArea.position.y + _touchArea.sizeDelta.y / 2f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
