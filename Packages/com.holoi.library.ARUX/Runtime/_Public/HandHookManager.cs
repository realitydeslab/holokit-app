using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHookManager : MonoBehaviour
{
    public Transform _targetJoint;

    public float _hookLength;

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        var eyePos = Camera.main.transform.position;

        var direction = _targetJoint.position - eyePos;

        direction = direction.normalized;

        //transform.position = _targetJoint.position;
        transform.position = _targetJoint.position + (direction * _hookLength);

        Debug.Log("DD" + direction);
        Debug.Log("DO" + direction * _hookLength);
        //Debug.Log("HT" + _handTips[1].position);
        //Debug.Log("FF" + transform.position);
    }
}
