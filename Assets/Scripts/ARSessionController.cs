using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARFoundation;

public class ARSessionController : MonoBehaviour
{
    public bool CoachingOverlaySupported
    {
        get
        {
            if (HoloKitHelper.IsRuntime)
            {
                return ARKitSessionSubsystem.coachingOverlaySupported;
            }
            else
            {
                return false;
            }
        }
    }

    private void Start()
    {
        if (CoachingOverlaySupported && GetComponent<ARSession>().subsystem is ARKitSessionSubsystem sessionSubsystem)
        {
            Debug.Log("Start coaching overlay");
            sessionSubsystem.requestedCoachingGoal = ARCoachingGoal.HorizontalPlane;
            sessionSubsystem.coachingActivatesAutomatically = true;
            sessionSubsystem.sessionDelegate = new CoachingOverlaySessionDelegate();
            sessionSubsystem.SetCoachingActive(true, ARCoachingOverlayTransition.Animated);
        }
    }
}
