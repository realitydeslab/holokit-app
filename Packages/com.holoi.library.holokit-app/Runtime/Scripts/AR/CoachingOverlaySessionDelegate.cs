using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;
using System;

namespace Holoi.Library.HoloKitApp
{
    public class CoachingOverlaySessionDelegate : DefaultARKitSessionDelegate
    {
        public static event Action OnCoachingOverlayViewStarted;

        public static event Action OnCoachingOverlayViewEnded;

        protected override void OnCoachingOverlayViewWillActivate(ARKitSessionSubsystem sessionSubsystem)
        {
            OnCoachingOverlayViewStarted?.Invoke();
        }

        protected override void OnCoachingOverlayViewDidDeactivate(ARKitSessionSubsystem sessionSubsystem)
        {
            OnCoachingOverlayViewEnded?.Invoke();
        }
    }
}
