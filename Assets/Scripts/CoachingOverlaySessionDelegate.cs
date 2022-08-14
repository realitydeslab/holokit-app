using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;
using System;

public class CoachingOverlaySessionDelegate : DefaultARKitSessionDelegate
{
    public static event Action OnCoachingOverlayViewStarted;

    public static event Action OnCoachingOverlayViewEnded;

    protected override void OnCoachingOverlayViewWillActivate(ARKitSessionSubsystem sessionSubsystem)
    {
        Debug.Log("Coaching overlay view started");
        OnCoachingOverlayViewStarted?.Invoke();
    }

    protected override void OnCoachingOverlayViewDidDeactivate(ARKitSessionSubsystem sessionSubsystem)
    {
        Debug.Log("Coaching overlay view ended");
        OnCoachingOverlayViewEnded?.Invoke();
    }
}
