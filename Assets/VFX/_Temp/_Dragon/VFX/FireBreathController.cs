using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreathController : MonoBehaviour
{
    public Transform _followPoint;

    void Start()
    {
        
    }

    void Update()
    {
        if(_followPoint != null)
        {
            transform.rotation = _followPoint.rotation;
            transform.position = _followPoint.position;
        }
    }
}
