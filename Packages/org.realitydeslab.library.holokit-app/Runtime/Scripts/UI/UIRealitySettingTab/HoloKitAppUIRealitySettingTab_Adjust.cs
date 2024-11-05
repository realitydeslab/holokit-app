// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTab_Adjust : HoloKitAppUIRealitySettingTab
    {
        public override string TabName => "Adjust";

        [SerializeField] private Image _dragImage;

        [SerializeField] private Image _twistImage;

        [SerializeField] private Image _pinchImage;

        public static event Action<Vector2> OnPositionChanged;

        public static event Action<float> OnRotationChanged;

        public static event Action<float> OnScaleChanged;

        private void Start()
        {
            if (!ARObjectAdjuster.Instance.Translation)
            {
                _dragImage.color = new(1f, 1f, 1f, 0.5f);
            }
            if (!ARObjectAdjuster.Instance.Rotation)
            {
                _twistImage.color = new(1f, 1f, 1f, 0.5f);
            }
            if (!ARObjectAdjuster.Instance.Scale)
            {
                _pinchImage.color = new(1f, 1f, 1f, 0.5f);
            }
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                // For dragging
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnPositionChanged?.Invoke(touch.deltaPosition);
                    }
                }
                else if (Input.touchCount == 2)
                {
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);
                    if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                    {
                        var lastTouchPosition1 = touch1.position - touch1.deltaPosition;
                        var lastTouchPosition2 = touch2.position - touch2.deltaPosition;
                        // For twisting
                        var lastDirection = lastTouchPosition2 - lastTouchPosition1;
                        var currentDirection = touch2.position - touch1.position;
                        var angle = Vector2.SignedAngle(lastDirection, currentDirection);
                        OnRotationChanged?.Invoke(angle);
                        // For pinching
                        float lastDistance = Vector2.Distance(lastTouchPosition1, lastTouchPosition2);
                        float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                        var factor = currentDistance / lastDistance;
                        OnScaleChanged?.Invoke(factor);
                    }
                } 
            }
        }
    }
}
