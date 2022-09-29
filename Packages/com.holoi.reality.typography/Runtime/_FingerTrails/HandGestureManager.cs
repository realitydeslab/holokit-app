using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class HandGestureManager : MonoBehaviour
{
    [SerializeField] Transform _wrist;
    [SerializeField] Transform[] _handTips = new Transform[5];
    [SerializeField] Transform[] _handSeconds = new Transform[5];
    [SerializeField] Transform[] _handThirds = new Transform[5];
    [SerializeField] Transform[] _handForth = new Transform[5];

    [SerializeField] Transform[] _smoothHandTips = new Transform[5];
    [SerializeField] Transform[] _smoothHandSeconds = new Transform[5];

    Vector3[] _tipNoramls = new Vector3[5];
    float[] _tipVelocity = new float[5];
    Vector3[] _tipVelocityDirection = new Vector3[5];
    Vector3[] _lastPosition = new Vector3[5];
    Vector3[] _currentPosition = new Vector3[5];

    [HideInInspector]
    public Vector3[] TipVelocityDirection
    { get { return _tipVelocityDirection; } }
    public Vector3[] TipNormals
    { get { return _tipNoramls; }}
    [HideInInspector]
    public Transform[] HandTips
    { get { return _handTips; } }
    public Transform[] HandSeconds
    { get { return _handSeconds; } }

    [HideInInspector] public enum Gesture
    {
        DK,
        Fist,
        Palm,
        Finger
    }

    public Gesture CurrentGesture = Gesture.DK;

    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        CalculateSmoothTipNormals();
        //CalculateTipNormals();
        // GestureDetection();
        CalculateTipVelocity();
    }

    void GestureDetection()
    {
        if (Application.isEditor)
        {

        }
        else
        {
            if (GetComponent<HoloKitHandTracker>().Valid)
            {
                var wrist = new Vector2(_wrist.position.x, _wrist.position.y);
                var tip = new Vector2(_handTips[2].position.x, _handTips[2].position.y);
                var third = new Vector2(_handThirds[2].position.x, _handThirds[2].position.y);

                var tipdist = Vector2.Distance(wrist, tip);
                var thirddist = Vector2.Distance(wrist, third);

                if (thirddist > tipdist)
                {
                    CurrentGesture = Gesture.Fist;
                }
                else
                {
                    CurrentGesture = Gesture.Palm;
                }
            }
            else
            {
                CurrentGesture = Gesture.Palm;
            }

        }

    }

    void CalculateTipNormals()
    {
        for (int i = 0; i < 5; i++)
        {
            _tipNoramls[i] = (_handTips[i].position - _handSeconds[i].position).normalized; 
        }
    }


    void CalculateTipVelocity()
    {
        // if(is first frame)
        // else

        for (int i = 0; i < 5; i++)
        {
            _currentPosition[i] = _handTips[i].position;

            var dist = Vector3.Distance(_currentPosition[i], _lastPosition[i]);
            _tipVelocityDirection[i] = (_currentPosition[i] - _lastPosition[i]).normalized;
            _tipVelocity[i] = dist / Time.deltaTime;

            _lastPosition[i] = _currentPosition[i];
        }
    }

    void CalculateSmoothTipNormals()
    {
        for (int i = 0; i < 5; i++)
        {
            _tipNoramls[i] = (_smoothHandTips[i].position - _smoothHandSeconds[i].position).normalized;
        }
    }
}