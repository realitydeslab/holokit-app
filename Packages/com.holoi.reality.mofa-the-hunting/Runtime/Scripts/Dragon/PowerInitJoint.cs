using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PowerInitJoint : MonoBehaviour
{
    
    void Start()
    {
        LookAtConstraint lac = GetComponent<LookAtConstraint>();
        ConstraintSource cs = new();
        cs.sourceTransform = HoloKit.HoloKitCamera.Instance.CenterEyePose;
        cs.weight = 1;
        lac.AddSource(cs);
        lac.constraintActive = true;
    }
}
