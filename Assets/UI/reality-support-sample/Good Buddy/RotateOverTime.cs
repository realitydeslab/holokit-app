using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float RotateSpeed;
    public Vector3 RotateVector;
    Vector3 _angle;
    Quaternion _offset;
    void Start()
    {
        _offset = transform.rotation;
    }
    void Update()
    {
        _angle += RotateVector * RotateSpeed * Time.deltaTime;
        transform.rotation = _offset *  Quaternion.Euler(_angle.x, _angle.y, _angle.z);
    }
}
