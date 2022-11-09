using System;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTab_Adjust : HoloKitAppUIRealitySettingTab
    {
        public override string TabName => "Adjust";

        private Vector2 _lastTouchPosition;

        public static event Action<Vector2> OnDragPositionChanged;

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                // For dragging
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        _lastTouchPosition = touch.position;
                        return;
                    }
                    else
                    {
                        OnDragPositionChanged?.Invoke(touch.position - _lastTouchPosition);
                        _lastTouchPosition = touch.position;
                    }
                }
            }
        }
    }
}
