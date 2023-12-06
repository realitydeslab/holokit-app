using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class AudioVFXManager : MonoBehaviour
{
    public Transform _handPositionSample;
    [SerializeField] private float multipier;

    private void Update()
    {
        Vector3 handOnPlane = Vector3.ProjectOnPlane(_handPositionSample.position, HoloKitCamera.Instance.CenterEyePose.forward);

        var dist = Vector2.Distance(Vector2.zero, new Vector2(handOnPlane.x, handOnPlane.y));

        transform.localScale = Vector3.one * dist * multipier;

        // Debug.Log($"handOnPlane{handOnPlane}");
        // Debug.Log($"dist{dist}");
    }
}